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
        ReleaseLog.LogInfo($"[testum]FinishFlyAway : (DamagedAirborneLoopState)");
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        // 엄todo : 이전 State에 따라서 fadeTime이 동적으로 바뀌어야 할 필요가 있음
        if (_character.GetPrevState() == eRoleState.FLY_AWAY_DAMAGED)
        {
            ReleaseLog.LogInfo("[testheight]FLY_AWAY_DAMAGED after");
            _moveSet.Play(_action, 1f);
        }
        else if (_character.GetPrevState() == eRoleState.AIRBORNE_DAMAGED)
        {
            // 이전 AIRBORNE_DAMAGED state의 애니메이션이 끝까지 재생 여부에 따른 분기
            if (_moveSet.IsAnimationFinish())
            {
                ReleaseLog.LogInfo("[testheight]maxHeight after");
                // 최대 높이 도달 후 로직
                _moveSet.Play(_action);
                _moveSet.SetSpeed(0.1f);
            }
            else
            {
                ReleaseLog.LogInfo("[testheight]not maxHeight after");
                // 에어본 도중 피격 시 처리
                //_moveSet.Play(_action, 100.0f);
            }
            _moveVelocity = Vector3.zero;
        }
        else
        {
            _moveVelocity = Vector3.zero;
        }
        _airTimer = 0f;
        _character.SetMaxHeightAirborne(false);
    }

    public override void FixedUpdateState()
    {
        if (_character.IsGround())
        {
            _character.ChangeRoleState(eRoleState.DAMAGED_LANDING, eStateType.DAMAGE_LANDING);
            return;
        }
        if (_character.GetPrevState() != eRoleState.FLY_AWAY_DAMAGED)
        {
            _airTimer += Time.fixedDeltaTime;
            _moveVelocity.y = -1f * _character.GetAirBoneDownVelocity(_airTimer, _character.GetGravityDownTime(), _character.GetGravityDownHeight());
            _character.SetVelocity(_moveVelocity);
        }
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_moveSet.IsAnimationFinish())
        {
            _moveSet.Play(_action, 10f);
            _moveSet.SetSpeed(0.1f);
        }
    }
}