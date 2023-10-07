using Animancer;
using UnityEngine;
using UnityEditor;

public class NormalDamagedState : DamagedState
{
    private AnimancerState _curState;

    public NormalDamagedState(Character character, eState eState) : base(character, eState)
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
        base.UpdateState();
    }
}