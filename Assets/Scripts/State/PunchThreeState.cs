using Animancer;
using UnityEngine;
using UnityEditor;

public class PunchThreeState : AttackState
{
    private AnimancerState _curState;

    public PunchThreeState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _curState = _action.Play();
        _character.ActiveAttackColliders(true, ActorHitColliderType.LEFT_HAND);
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            if (_action.IsFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
        }
    }
}