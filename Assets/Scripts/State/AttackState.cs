using UnityEngine;
using UnityEditor;

public class AttackState : State
{
    public AttackState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        animator.Play("Punch");
        // animator.CrossFade("X_Punch", character.attackStart);
        // animator.CrossFade("Attack", character.attackStart);
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                character.ChangeState(eState.IDLE);
            }
        }
    }
}