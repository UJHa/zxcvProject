using Animancer;
using UnityEngine;
using UnityEditor;

public class KickTwoState : AttackState
{
    private AnimancerState _curState;

    public KickTwoState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _curState = _action.Play();
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
            if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
            
            bool collisionEnable = _action.IsCollisionEnable();
            _character.ActiveAttackColliders(collisionEnable, _action.GetHitColliderType());
        }
    }
}