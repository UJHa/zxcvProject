using Animancer;
using UnityEngine;
using UnityEditor;

public class RunState : State
{
    public RunState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.SetMoveSpeedToRun();
        _action.Play(_character.runStart);
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            _character.ChangeState(eState.JUMP_DOWN);
        }
        else
            _character.UpdateGroundHeight();
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
            _character.ChangeState(eState.RUN_STOP);
            return;
        }

        if (InputManager.Instance.GetButtonDown(KeyBindingType.JUMP))
        {
            _character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
            return;
        }
        
        _character.SetDirectionByVector3(vector);
        _character.MovePosition(vector);
    }
}