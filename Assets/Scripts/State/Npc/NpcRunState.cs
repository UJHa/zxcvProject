using UnityEngine;
using UnityEditor;

public class NpcRunState : RunState
{
    public NpcRunState(Character character, eState eState) : base(character, eState)
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
            Vector3 traceDirection = (_character.GetTraceTarget().transform.position - _character.transform.position).normalized;
            Debug.Log($"[botRotVector]{traceDirection}");
            traceDirection.y = 0f;
            _character.SetDirectionByVector3(traceDirection);
            _character.MovePosition(traceDirection);
            
            // if (Vector3.Distance(_character.GetTraceTarget().transform.position, _character.transform.position) <= _attackRange)
            if (Vector3.Distance(_character.GetTraceTarget().transform.position, _character.transform.position) <= 1f)
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }
            if (_character.IsInRange())
            {
                // _character.SetTarget(null);
                _character.ChangeRoleState(eRoleState.IDLE);
            }
        }
        else
        {
            _character.ChangeRoleState(eRoleState.IDLE);
        }
    }
}