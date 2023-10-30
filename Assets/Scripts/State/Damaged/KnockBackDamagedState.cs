using System.Diagnostics;
using UnityEngine;

public class KnockBackDamagedState : DamagedState
{
    private float _knockBackDeltaTime = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
    private long _knockBackTimeMilliSec = 1000;
    private float _knockBackTimeSec = 1f;
    
    public KnockBackDamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }
    
    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        _knockBackDeltaTime = 0f;
    }

    public override void FixedUpdateState()
    {
        _knockBackDeltaTime += Time.fixedDeltaTime;
        _moveVelocity = _character.GetDamagedDirectionVector() * _character.GetKnockBackVelocity(_knockBackDeltaTime, _knockBackTimeSec, 1f);
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
        
        if (_knockBackTimeSec <= _knockBackDeltaTime)
            _character.ChangeRoleState(eRoleState.IDLE);
    }
}