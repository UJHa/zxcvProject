using System.Diagnostics;
using DataClass;
using UnityEngine;

public class NpcLandingState : LandingState
{
    public NpcLandingState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
        
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
        // base.UpdateState();
        if (IsAnimationFinish())
            _character.ChangeRoleState(eRoleState.IDLE);
    }
}