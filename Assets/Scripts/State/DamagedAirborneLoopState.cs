using UnityEngine;
using UnityEditor;

public class DamagedAirborneLoopState : DamagedState
{
    private float _airTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public DamagedAirborneLoopState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _moveSet.Play(_action, 1f);
        _airTimer = 0f;
    }

    public override void FixedUpdateState()
    {
        _airTimer += Time.fixedDeltaTime;
        // _moveVelocity.y = -1f * _character.GetAirBoneDownVelocity(_airTimer, _character.GetAttackedAirborneUpTime(), _character.GetAttackedMaxHeight());
        _moveVelocity.y = -1f * _character.GetAirBoneDownVelocity(_airTimer, _character.GetGravityDownTime(), _character.GetGravityDownHeight());
        _character.SetVelocity(_moveVelocity);
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}