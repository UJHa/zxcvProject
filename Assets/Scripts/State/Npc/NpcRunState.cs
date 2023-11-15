using UnityEngine;
using UnityEditor;

public class NpcRunState : RunState
{
    public NpcRunState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        
        _character.SetMoveSpeedToRun();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
        if(_character.GetTraceTarget() != null)
        {
            _character.RotateToPosition(_character.GetTraceTarget().transform.position);
            var moveVelocity = _character.ComputeMoveVelocityXZ(_character.GetDirectionVector());
            _character.SetVelocity(moveVelocity);
            
            if (_character.GetTraceTargetDistanceXZ() <= _character.GetAttackStartDistance())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }
        }
        else
        {
            _character.ChangeRoleState(eRoleState.IDLE);
        }
    }
}