using UnityEngine;
using UnityEditor;

public class PunchOneState : AttackState
{
    private string _animName = "Punch1";
    public PunchOneState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        _animator.Play(_animName);
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animName))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                _character.ChangeState(eState.ATTACK2);
            }
        }
    }
}