using Animancer;
using UnityEngine;

public class DamagedState : State
{
    private AnimancerState _curState;
    public DamagedState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _curState = _action.Play();
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
            else
            {
                var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.X);
                if (eState.NONE != nextState2 && _eState == nextState2)
                    _animancer.States.Current.Time = 0f;
            }
        }
    }
}