using Animancer;
using UnityEngine;
using UnityEditor;

public class DamagedAirborneState : DamagedState
{
    private AnimancerState _curState;

    public DamagedAirborneState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _curState = _action.Play(1f);
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
                _character.ChangeState(eState.DAMAGED_LANDING);
            }
            else
            {
                var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.Z);
                if (eState.NONE != nextState2 && _eState == nextState2)
                    _animancer.States.Current.Time = 0f;
            }
        }
    }
}