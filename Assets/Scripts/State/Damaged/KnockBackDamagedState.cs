using System.Diagnostics;
using UnityEngine;

public class KnockBackDamagedState : DamagedState
{
    private Vector3 _moveVelocity = Vector3.zero;
    private Stopwatch _knockBackTimer = new();
    private long _knockBackTimeMilliSec = 1000;
    
    public KnockBackDamagedState(Character character, eState eState) : base(character, eState)
    {
    }
    
    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        _knockBackTimer.Start();
    }

    public override void FixedUpdateState()
    {
        _moveVelocity = _character.GetDamagedDirectionVector() * 1f;
        _character.SetVelocity(_moveVelocity);
    }

    public override void EndState()
    {
        base.EndState();
        _knockBackTimer.Reset();
        _character.SetVelocity(_moveVelocity);
        _character.SetDamagedDirectionVector(Vector3.zero);
    }

    public override void UpdateState()
    {
        if (_moveSet.IsAnimationFinish())
            _moveSet.PauseAnimation();
        
        if (_knockBackTimer.ElapsedMilliseconds >= _knockBackTimeMilliSec)
            _character.ChangeRoleState(eRoleState.IDLE);
    }
}