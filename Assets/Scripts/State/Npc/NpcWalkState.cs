using UnityEngine;
using UnityEditor;

public class NpcWalkState : WalkState
{
    public NpcWalkState(Character character, eState eState) : base(character, eState)
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
            _character.MovePosition(traceDirection);
            
            if (_character.GetTraceTargetDistanceXZ() >= 3f)
            {
                _character.ChangeRoleState(eRoleState.RUN);
            }
            else if (_character.GetTraceTargetDistanceXZ() <= 1f)
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