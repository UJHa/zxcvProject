using UnityEngine;

public class DamagedLandingState : DamagedState
{

    public DamagedLandingState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(false, HitColliderType.AIRBORNE);
        _moveSet.Play(_action, 0.3f);
        _character.SetVelocity(Vector3.zero);
        _character.UpdateGroundHeight(true);
        _character._isGround = true;
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            if (_moveSet.IsAnimationFinish())
            {
                if (_character.IsDead())
                    _character.ChangeState(eState.DEAD);
                else
                    _character.ChangeState(eState.WAKE_UP);
            }
        }
    }
}