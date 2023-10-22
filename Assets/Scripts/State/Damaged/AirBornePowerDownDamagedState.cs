using UnityEngine;

public class AirBornePowerDownDamagedState : DamagedState
{
    private Vector3 _moveVelocity = Vector3.zero;
    private float _maxUpHeight = 0f;

    public AirBornePowerDownDamagedState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _moveSet.Play(_action);
        _character._isGround = false;
    }

    public override void FixedUpdateState()
    {
        _moveVelocity.y = -15f;
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