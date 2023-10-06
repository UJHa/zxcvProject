using UnityEngine;

public class AirAttackThree : AttackState
{
    public AirAttackThree(Character character, eState eState) : base(character, eState)
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
        if (_action.IsAnimationFinish())
        {
            _character.ChangeState(eState.JUMP_DOWN);
        }
        
        bool collisionEnable = _action.IsCollisionEnable();
        _character.ActiveAttackCollider(collisionEnable, _action.GetHitColliderType(), _action.GetaAttackInfo());
    }
}