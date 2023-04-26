using UnityEngine;
using UnityEditor;

public class ComboState : State
{
    private int maxComboCount = 3;
    private int curComboCount = 1;
    public ComboState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        curComboCount = 1;
        animator.Play($"Punch{curComboCount}");
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Punch1")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Punch2")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Punch3"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                character.ChangeState(eState.IDLE);
            }
            else if (Input.GetKeyDown(KeyCode.C) && character.IsGround() && curComboCount < maxComboCount)
            {
                animator.Rebind();
                curComboCount++;
                animator.Play($"Punch{curComboCount}");
            }
        }
    }
}