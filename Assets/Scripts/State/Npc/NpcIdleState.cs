using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class NpcIdleState : IdleState
{
    public NpcIdleState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.SetTarget(GameManager.Instance.mainPlayer);
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
        if (Vector3.Distance(_character.GetTraceTarget().transform.position, _character.transform.position) > 1f)
        {
            _character.ChangeRoleState(eRoleState.RUN);
        }
        // base.UpdateState();
    }
}