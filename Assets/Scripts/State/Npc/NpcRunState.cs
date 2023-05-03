using UnityEngine;
using UnityEditor;

public class NpcRunState : State
{
    private bool _isJump = false;
    private float _attackRange = 1.0f;

    public NpcRunState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        _isJump = false;

        _character.SetMoveSpeedToRun();
        _animator.Play("Run");
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
        if(_character.GetTraceTarget() != null)
        {
            _character.transform.LookAt(_character.GetTraceTarget().transform);
            Vector3 temp = _character.transform.eulerAngles;
            temp.x = 0.0f;
            _character.transform.eulerAngles = temp;
            Vector3 traceDirection = (_character.GetTraceTarget().transform.position - _character.transform.position).normalized;
            _character.transform.position += traceDirection * _character.GetMoveSpeed();
            if (Vector3.Distance(_character.GetTraceTarget().transform.position, _character.transform.position) <= _attackRange)
            {
                _character.ChangeState(eState.ATTACK);
            }
            if (_character.IsInRange())
            {
                _character.SetTarget(null);
                _character.ChangeState(eState.IDLE);
            }
        }
        else
        {
            _character.ChangeState(eState.IDLE);
        }
    }
}