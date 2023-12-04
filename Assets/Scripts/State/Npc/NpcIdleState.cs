using UnityEngine;
using UnityEditor;

public class NpcIdleState : IdleState
{
    public NpcIdleState(Character character, ActionKey actionKey) : base(character, actionKey)
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
        //if (_character.GetTraceTargetDistanceXZ() > _character.GetAttackStartDistance() + _character.GetWalkTraceDistance())
        //{
        //    _character.ChangeRoleState(eRoleState.RUN);
        //}
        //else if (_character.GetTraceTargetDistanceXZ() > _character.GetAttackStartDistance())
        //{
        //    _character.ChangeRoleState(eRoleState.WALK);
        //}
        //else
        //{
        //    _character.RotateToPosition(_character.GetTraceTarget().transform.position);
        //    _character.ChangeRoleState(eRoleState.WEEK_ATTACK1);
        //}
    }
}