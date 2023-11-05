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
            Vector3 traceDirection = (_character.GetTraceTarget().transform.position - _character.transform.position).normalized;
            traceDirection.y = 0f;
            _character.SetDirectionByVector3(traceDirection);
            var moveVelocity = _character.ComputeMoveVelocityXZ(traceDirection);
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