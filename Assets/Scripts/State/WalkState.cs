using System;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class WalkState : State
{
    public WalkState(Character character) : base(character)
    {
    }

    public override void StartState()
    {

        character.SetMoveSpeedToWalk();

        animator.CrossFade("Walk", character.walkStart);
    }

    public override void FixedUpdateState()
    {
        if (false == character.IsGroundCheck())
        {
            character.ChangeState(eState.JUMP_DOWN);
        }
        else
            character.UpdateGroundHeight();
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
        UpdateInput();
    }
    
    void UpdateInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero == vector)
        {
            character.ChangeState(eState.IDLE);
            return;
        }

        if (InputManager.Instance.GetButtonDown(KeyBindingType.JUMP))
        {
            character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
            return;
        }
        
        character.SetDirectionByVector3(vector);
        character.MovePosition(vector);
    }
}