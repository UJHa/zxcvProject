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
        Debug.Log($"[testum]FinishFlyAway : (DamagedAirborneLoopState)");
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        // 엄todo : 이전 State에 따라서 fadeTime이 동적으로 바뀌어야 할 필요가 있음
        // 현재 : AirborneDamagedState : 1초
        // 현재 : FlyAwayDamagedState : 0.1초
        _moveSet.Play(_action, 0.1f);
        _airTimer = 0f;
    }

    public override void FixedUpdateState()
    {
        _airTimer += Time.fixedDeltaTime;
        _moveVelocity.y = -1f * _character.GetAirBoneDownVelocity(_airTimer, _character.GetGravityDownTime(), _character.GetGravityDownHeight());
        _character.SetVelocity(_moveVelocity);
        {
            var groundObjs = _character.GetGroundCheckObjects();
            if (0 != groundObjs.Length)
                _character.OnAirborneLanding();
        }
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}