using DataClass;
using UnityEngine;

public class DamagedLandingState : DamagedState
{
    public DamagedLandingState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(false, HitColliderType.AIRBORNE);
        AnimationFadeInfoData data = _character.GetAnimFadeInfoData();
        _moveSet.Play(_action, data.damageLandingStart);//0.1f
        _character.SetVelocity(Vector3.zero);
        _character.UpdateGroundHeight(true);
        _character.SetGrounding(true);
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
                    _character.ChangeState(eRoleState.DEAD);
                else
                    _character.ChangeState(eRoleState.WAKE_UP);
            }
        }
    }
}