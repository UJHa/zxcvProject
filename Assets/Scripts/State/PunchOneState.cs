using Animancer;
using UnityEngine;
using UnityEditor;

public class PunchOneState : AttackState
{
    private AnimancerState _curState;

    public PunchOneState(Character character, eState eState) : base(character, eState)
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
        base.EndState();
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.C);
            if (eState.NONE != nextState)
                _character.ChangeState(nextState, eStateType.INPUT);
            else if (_curState.NormalizedTime > 0.7f)
            {
                _character.ChangeState(eState.IDLE);
            }
        }
    }
}