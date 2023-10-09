using UnityEngine;

public class AirAttackThreeState : AttackState
{
    public AirAttackThreeState(Character character, eState eState) : base(character, eState)
    {

    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
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
        if (_moveSet.IsAnimationFinish())
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }
        
        bool collisionEnable = _moveSet.IsCollisionEnable();
        _character.ActiveAttackCollider(collisionEnable, _action.GetHitColliderType(), _action.GetaAttackInfo());
    }
}