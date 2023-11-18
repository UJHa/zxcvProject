using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected HashSet<int> _instanceIds = new();
    public AttackState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _instanceIds.Clear();
        _character.ResetMoveSpeed();
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
    }
}