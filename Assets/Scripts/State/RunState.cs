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
        animator.Play("Run");
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