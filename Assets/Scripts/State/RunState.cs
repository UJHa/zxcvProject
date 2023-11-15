using DataClass;
using UnityEngine;

public class RunState : State
{
    public RunState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.SetMoveSpeedToRun();
        AnimationFadeInfoData data = _character.GetAnimFadeInfoData();
        _moveSet.Play(_action, data.runStart);
    }

    public override void FixedUpdateState()
    {
        if (!_character.IsGround())
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
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
            _character.ChangeRoleState(eRoleState.RUN_STOP);
            return;
        }

        var nextState = _moveSet.DetermineNextState(_character.GetCurState());
        if (eRoleState.NONE != nextState)
        {
            _character.ChangeRoleState(nextState, eStateType.INPUT);
            return;
        }
        
        _character.SetDirectionByVector3(vector);
        var moveVelocity = _character.ComputeMoveVelocityXZ(vector);
        _character.SetVelocity(moveVelocity);
    }
}