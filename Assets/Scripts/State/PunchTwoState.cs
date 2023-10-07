using Animancer;
using UnityEngine;
using UnityEditor;

public class PunchTwoState : AttackState
{
    private AnimancerState _curState;

    public PunchTwoState(Character character, eState eState) : base(character, eState)
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
            var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyBindingType.WEEK_ATTACK);
            if (eState.NONE != nextState)
                _character.ChangeState(nextState, eStateType.INPUT);
            else if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
        }
        bool collisionEnable = _action.IsCollisionEnable();
        _character.ActiveAttackCollider(collisionEnable, _action.GetHitColliderType(), _action.GetaAttackInfo());
    }
}