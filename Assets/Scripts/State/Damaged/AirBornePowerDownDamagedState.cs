using UnityEngine;

public class AirBornePowerDownDamagedState : DamagedState
{
    private float _airTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

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
        _airTimer += Time.fixedDeltaTime;
        _moveVelocity.y = -1f * _character.GetAirBoneDownVelocity(_airTimer, 0.1f, 1f);
        _character.SetVelocity(_moveVelocity);
        {
            var groundObjs = _character.GetGroundCheckObjects();
            if (0 != groundObjs.Length)
                _character.OnAirborneLanding();
        }
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
    }
}