using UnityEngine;

public class AirBornePowerDownDamagedState : DamagedState
{
    private float _airTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public AirBornePowerDownDamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _moveSet.Play(_action);
        _character.SetGrounding(false);
        _moveVelocity = Vector3.zero;
    }

    public override void FixedUpdateState()
    {
        if (_character.IsGround())
        {
            _character.ChangeRoleState(eRoleState.DAMAGED_LANDING, eStateType.DAMAGE_LANDING);
            _character.SetVelocity(Vector3.zero);
            return;
        }
        _airTimer += Time.fixedDeltaTime;
        _moveVelocity.y = -1f * _character.GetAirBoneDownVelocity(_airTimer, 0.1f, 1f);
        _character.SetVelocity(_moveVelocity);
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
    }
}