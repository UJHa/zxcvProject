using Animancer;
using UnityEngine;
using UnityEditor;

public class PunchThreeState : AttackState
{
    private AnimancerState _curState;

    public PunchThreeState(Character character, eState eState) : base(character, eState)
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
            if (_curState.NormalizedTime > 0.4f)
            {
                _character.ChangeState(eState.IDLE);
            }
        }
    }
}