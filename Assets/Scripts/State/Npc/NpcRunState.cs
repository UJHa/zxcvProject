using UnityEngine;
using UnityEditor;

public class NpcRunState : State
{
    private bool _isJump = false;
    private float _attackRange = 1.0f;

    public NpcRunState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _isJump = false;

        character.SetMoveSpeedToRun();
        animator.SetBool("Run", true);
    }

    public override void EndState()
    {
        if (!_isJump)
            animator.SetBool("Run", false);
    }

    public override void UpdateState()
    {
        if(character.GetTraceTarget() != null)
        {
            character.transform.LookAt(character.GetTraceTarget().transform);
            Vector3 temp = character.transform.eulerAngles;
            temp.x = 0.0f;
            character.transform.eulerAngles = temp;
            Vector3 traceDirection = (character.GetTraceTarget().transform.position - character.transform.position).normalized;
            character.transform.position += traceDirection * character.GetMoveSpeed();
            if (Vector3.Distance(character.GetTraceTarget().transform.position, character.transform.position) <= _attackRange)
            {
                character.ChangeState(eState.ATTACK);
            }
            if (character.IsInRange())
            {
                character.SetTarget(null);
                character.ChangeState(eState.IDLE);
            }
        }
        else
        {
            character.ChangeState(eState.IDLE);
        }
    }
}