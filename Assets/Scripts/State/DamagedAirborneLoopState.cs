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
        _character.ClearAttackInfoData();
    }

    public override void FixedUpdateState()
    {
        _airTimer += Time.fixedDeltaTime;
        _moveVelocity.y = _character.GetAirBoneDownVelocity(_airTimer);
        _character.GetRigidbody().velocity = _moveVelocity;
        Debug.Log($"[damagedown]timer({_airTimer}) GetVelocity({_character.GetAirBoneUpVelocity(_airTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}