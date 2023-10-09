using System.Diagnostics;

public class RunStopState : State
{
    private Stopwatch _inputTimer;
    private float _jumpTimer = 0f;
    private long _stopTimeMSec;
    private long _remainTime;
    private float _remainRate;

    public RunStopState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        base.StartState();
        _character.SetMoveSpeedToRun();
        _inputTimer.Start();
        _moveSet.Play(_action);
        _stopTimeMSec = (long)(_moveSet.GetClipLength() * (1f / _moveSet.GetClipSpeed()) * 1000);
        _remainTime = _stopTimeMSec;
        _remainRate = (float)_remainTime / _stopTimeMSec;
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }
        else
            _character.MovePosition(_character.GetDirectionVector(), _character.GetMoveSpeed() * (_remainRate / 2f));
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        _remainTime = _stopTimeMSec - _inputTimer.ElapsedMilliseconds;
        _remainRate = (float)_remainTime / _stopTimeMSec;
        if (0 >= _remainTime)
        {
            _character.ChangeRoleState(eRoleState.IDLE);
            return;
        }
    }
}