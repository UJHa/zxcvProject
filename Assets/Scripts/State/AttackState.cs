using UnityEngine;
using UnityEditor;

public class AttackState : State
{
    public AttackState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        animator.SetBool("Attack", true);
    }

    public override void EndState()
    {
        animator.SetBool("Attack", false);
    }

    public override void UpdateState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                character.ChangeState(eState.IDLE);
            }
        }
    }
}