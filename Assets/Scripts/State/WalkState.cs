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

        animator.Play("Walk");
    }

    public override void FixedUpdateState()
    {
        
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