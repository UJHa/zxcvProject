using UnityEngine;
using UnityEditor;

public class NpcWalkState : WalkState
{
    public NpcWalkState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
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
            
            if (_character.GetTraceTargetDistanceXZ() > _character.GetAttackStartDistance() + _character.GetWalkTraceDistance())
            {
                _character.ChangeRoleState(eRoleState.RUN);
            }
            else if (_character.GetTraceTargetDistanceXZ() <= _character.GetAttackStartDistance())
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