using UnityEngine;

public class FlyAwayDamagedState : DamagedState
{
    private float _flyAwayDeltaTime = 0f;
    private float _flyAwayTimeSec = 0.8f;
    private float _flyAwayGroundDistance = 2f;
    private float _flyAwayHeightDistance = 1f;
    
    private Vector3 _moveVelocity = Vector3.zero;
    
    public FlyAwayDamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }
    
    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        _flyAwayDeltaTime = 0f;
        _character._isGround = false;
        var animSpeed = _moveSet.GetClipLength() / _flyAwayTimeSec;
        _moveSet.SetSpeed(animSpeed);
        Debug.Log($"[testum][FlyAway]ClipLength({_moveSet.GetClipLength()})animSpeed({animSpeed})");
    }

    public override void FixedUpdateState()
    {
        _flyAwayDeltaTime += Time.fixedDeltaTime;
        _moveVelocity = _character.GetDamagedDirectionVector() * _character.GetFlyAwayGroundVelocity(_flyAwayDeltaTime, _flyAwayTimeSec, _flyAwayGroundDistance);
        _moveVelocity.y = _character.GetFlyAwayHeightVelocity(_flyAwayDeltaTime, _flyAwayTimeSec, _flyAwayHeightDistance);
        _character.SetVelocity(_moveVelocity);
    }

    public override void EndState()
    {
        base.EndState();
        // _knockBackTimer.Reset();
        _character.SetVelocity(_moveVelocity);
        _character.SetDamagedDirectionVector(Vector3.zero);
    }

    public override void UpdateState()
    {
        if (_moveSet.IsAnimationFinish())
            _moveSet.PauseAnimation();

        if (_flyAwayTimeSec <= _flyAwayDeltaTime)
            FinishFlyAway();
    }

    public void FinishFlyAway()
    {
        // _character.ActiveHitCollider(false, HitColliderType.STAND);
        // _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        var groundObjs = _character.GetGroundCheckObjects();
        Debug.Log($"[testum]FinishFlyAway : groundLength({groundObjs.Length})");
        if (0 == groundObjs.Length)
        {
            _character.ChangeState(eRoleState.DAMAGED_AIRBORNE_LOOP);
        }
        else
        {
            _character.ChangeState(eRoleState.DAMAGED_LANDING);
        }
    }
}