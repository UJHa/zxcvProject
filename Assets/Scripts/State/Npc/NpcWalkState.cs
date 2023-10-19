using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class NpcWalkState : WalkState
{
    public NpcWalkState(Character character, eState eState) : base(character, eState)
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
        // _character.ChangeRoleState(eRoleState.RUN);
        // base.UpdateState();
    }
}