using Animancer;
using UnityEngine;
using UnityEditor;

public class PunchOneState : AttackState
{
    private AnimationClip _animClip;
    private AnimancerState _curState;

    public PunchOneState(Character character, eState eState) : base(character, eState)
    {
        _animClip = Resources.Load<AnimationClip>("Animation/Lucy_FightFist01_1");
    }

    public override void StartState()
    {
        base.StartState();
        _curState = _animancer.Play(_animClip);
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