using UnityEngine;

public class AirAttackTwoState : AttackState
{
    public AirAttackTwoState(Character character, eState eState) : base(character, eState)
    {

    }

    public override void StartState()
    {
        base.StartState();
        _action.Play();
        _character.GetRigidbody().velocity = Vector3.zero;
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {

    }

    public override void UpdateState()
    {
        var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyBindingType.WEEK_ATTACK);
        if (eState.NONE != nextState)
            _character.ChangeState(nextState, eStateType.INPUT);
        else if (_action.IsAnimationFinish())
        {
            _character.ChangeState(eState.JUMP_DOWN);
        }
        
        bool collisionEnable = _action.IsCollisionEnable();
        _character.ActiveAttackCollider(collisionEnable, _action.GetHitColliderType(), _action.GetaAttackInfo());
    }
}