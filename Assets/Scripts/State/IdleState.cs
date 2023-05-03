using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class IdleState : State
{
    private Stopwatch _inputTimer;
    private long _inputDelayMSec = 150;
    public IdleState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        _character.ResetMoveSpeed();
        _character._isGround = true;
        // animator.enabled = true;
        _animator.CrossFade("Idle", _character.idleStart);
        _inputTimer.Start();

        // Idle도중 움직임이 없으므로 UpdateGroundHeight는 시작 시점 한 번만 처리
        _character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            Debug.Log("[testumAir]is not Ground!");
            _character.ChangeState(eState.JUMP_DOWN);
        }
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        UpdateAnimation();
        UpdateInput();
    }

    private void UpdateAnimation()
    {
        var curStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (curStateInfo.IsName("JumpEnd"))
            if (curStateInfo.normalizedTime >= 1.0f)
                _animator.Play("Idle");
    }

    private void UpdateInput()
    {
        if (!InputManager.IsExistInstance)
            return;
        
        UpdateMoveInput();

        if (_character.IsGround())
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                _character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
            }
            // else if (Input.GetKeyDown(KeyCode.C))
            else
            {
                _character.StartAction(KeyCode.C);
            }
        }
    }
    
    private void UpdateMoveInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero != vector)
        {
            if (eState.WALK == _character.GetPrevState()
                && vector == _character.GetDirectionVector()
                && _inputTimer.ElapsedMilliseconds <= _inputDelayMSec)
            {
                _character.ChangeState(eState.RUN);
            }
            else
            {
                _character.ChangeState(eState.WALK);
            }
        }
    }
}