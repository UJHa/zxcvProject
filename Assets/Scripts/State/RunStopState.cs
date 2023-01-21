using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class RunStopState : State
{
    private Stopwatch _inputTimer;
    private long _inputDelayMSec = 500;
    public RunStopState(Character character) : base(character)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        _inputTimer.Start();
        // character.ResetMoveSpeed();
        // animator.CrossFade("JumpEnd", character.jumpEnd);
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        if (_inputTimer.ElapsedMilliseconds >= _inputDelayMSec)
        {
            character.ChangeState(eState.IDLE);
            return;
        }
        
        UpdateAnimation();
        character.MovePosition(character.GetDirectionVector());
    }

    private void UpdateAnimation()
    {
        animator.enabled = false;
    }
}