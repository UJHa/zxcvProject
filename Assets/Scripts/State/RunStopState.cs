using System.Diagnostics;
using System.Linq;
using Animancer;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class RunStopState : State
{
    private Stopwatch _inputTimer;
    private float _jumpTimer = 0f;
    private long _stopingTimeMSec = 500;
    private long _remainTime;
    private float _remainRate;

    public RunStopState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        base.StartState();
        _inputTimer.Start();
        _remainTime = _stopingTimeMSec;
        _remainRate = (float)_remainTime / _stopingTimeMSec;
        _action.Play();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            _character.ChangeState(eState.JUMP_DOWN);
        }
        else
            _character.MovePosition(_character.GetDirectionVector(), _character.GetMoveSpeed() * _remainRate);
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        _remainTime = _stopingTimeMSec - _inputTimer.ElapsedMilliseconds;
        _remainRate = (float)_remainTime / _stopingTimeMSec;
        if (0 >= _remainTime)
        {
            _character.ChangeState(eState.IDLE);
            return;
        }
    }
}