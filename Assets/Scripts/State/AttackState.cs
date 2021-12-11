using UnityEngine;
using UnityEditor;

public class AttackState : State
{
    private GameObject collider;
    public AttackState(Character character) : base(character)
    {
        collider = character.transform.GetChild(2).gameObject;
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