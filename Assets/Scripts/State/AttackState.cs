using UnityEngine;
using UnityEditor;

public class AttackState : State
{
    public AttackState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        _animator.Play("Punch");
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
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                _character.ChangeState(eState.IDLE);
            }
        }
    }
}