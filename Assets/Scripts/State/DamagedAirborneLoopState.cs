using UnityEngine;
using UnityEditor;

public class DamagedAirborneLoopState : DamagedState
{
    private float _airTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public DamagedAirborneLoopState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        Debug.Log($"[testum]FinishFlyAway : (DamagedAirborneLoopState)");
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        // 엄todo : 이전 State에 따라서 fadeTime이 동적으로 바뀌어야 할 필요가 있음
        if (_character.GetPrevState() == eRoleState.FLY_AWAY_DAMAGED)
            _moveSet.Play(_action, 1f);
        else if (_character.GetPrevState() == eRoleState.AIRBORNE_DAMAGED)
            _moveSet.Play(_action, 0.1f);
        _airTimer = 0f;
        _moveVelocity = Vector3.zero;
    }

    public override void FixedUpdateState()
    {
        if (_character.IsGround())
        {
            _character.ChangeRoleState(eRoleState.DAMAGED_LANDING, eStateType.DAMAGE_LANDING);
            return;
        }
        _airTimer += Time.fixedDeltaTime;
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