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
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        _character.RefreshHitBoxKey();
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
        }
    }
}