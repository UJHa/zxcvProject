using UnityEngine;
using UnityEditor;

public class RunState : State
{
    public RunState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        character.SetMoveSpeedToRun();
        animator.CrossFade("Run", character.runStart);
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
            character.ChangeState(eState.RUNSTOP);
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