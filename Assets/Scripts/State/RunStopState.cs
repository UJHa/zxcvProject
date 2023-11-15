using System.Diagnostics;

public class RunStopState : State
{
    private Stopwatch _inputTimer;
    private float _jumpTimer = 0f;
    private long _stopTimeMSec;
    private long _remainTime;
    private float _remainRate;

    public RunStopState(Character character, ActionKey actionKey) : base(character, actionKey)
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
        if (!_character.IsGround())
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }
        else
        {
            var moveVelocity = _character.ComputeMoveVelocityXZ(_character.GetDirectionVector());
            moveVelocity *= (_remainRate / 2f);
            _character.SetVelocity(moveVelocity);
        }
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