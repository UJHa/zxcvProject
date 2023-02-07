using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class IdleState : State
{
    private Stopwatch _inputTimer;
    private long _inputDelayMSec = 150;
    public IdleState(Character character) : base(character)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        character.ResetMoveSpeed();
        character._isGround = true;
        animator.enabled = true;
        animator.CrossFade("Idle", character.idleStart);
        _inputTimer.Start();

        // Idle도중 움직임이 없으므로 UpdateGroundHeight는 시작 시점 한 번만 처리
        character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            Debug.Log("[testumAir]is not Ground!");
            character.ChangeState(eState.JUMP_DOWN);
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
        var curStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (curStateInfo.IsName("JumpEnd"))
            if (curStateInfo.normalizedTime >= 1.0f)
                animator.Play("Idle");
    }

    private void UpdateInput()
    {
        if (!InputManager.IsExistInstance)
            return;
        
        UpdateMoveInput();

        if(Input.GetKeyDown(KeyCode.V) && character.IsGround())
        {
            character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
        }

        if (Input.GetKeyDown(KeyCode.C) && character.IsGround())
        {
            character.ChangeState(eState.ATTACK);
        }
    }
    
    private void UpdateMoveInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero != vector)
        {
            if (eState.WALK == character.GetPrevState()
                && vector == character.GetDirectionVector()
                && _inputTimer.ElapsedMilliseconds <= _inputDelayMSec)
            {
                character.ChangeState(eState.RUN);
            }
            else
            {
                character.ChangeState(eState.WALK);
            }
        }
    }
}