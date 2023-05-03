using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class LandingState : State
{
    private Stopwatch _inputTimer;
    private long _inputDelayMSec = 150;
    public LandingState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        _character.ResetMoveSpeed();
        _character._isGround = true;
        _animator.CrossFade("JumpEnd", _character.jumpEnd);
        _inputTimer.Start();

        // Idle도중 움직임이 없으므로 UpdateGroundHeight는 시작 시점 한 번만 처리
        _character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        
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
                _character.ChangeState(eState.IDLE);
    }

    private void UpdateInput()
    {
        if (!InputManager.IsExistInstance)
            return;
        
        UpdateMoveInput();
        
        if(Input.GetKeyDown(KeyCode.V) && _character.IsGround())
        {
            _character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
        }
        
        if (Input.GetKeyDown(KeyCode.C) && _character.IsGround())
        {
            _character.ChangeState(eState.ATTACK);
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