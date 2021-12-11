using UnityEngine;
using UnityEditor;

public class NpcRunState : State
{
    private bool _isJump = false;

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
            character.transform.position += (character.GetTraceTarget().transform.position - character.transform.position).normalized * character.GetMoveSpeed();
            if (Vector3.Distance(character.GetTraceTarget().transform.position, character.transform.position) <= 1.0f)
            {
                character.ChangeState(eState.ATTACK);
            }
            if (Vector3.Distance(character.GetTraceTarget().transform.position, character.transform.position) > 3.0f)
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