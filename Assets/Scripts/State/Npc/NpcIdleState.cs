using UnityEngine;
using UnityEditor;

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
        if (_character.GetTraceTargetDistanceXZ() > 4f)
        {
            _character.ChangeRoleState(eRoleState.JUMP_UP);
        }
        else if (_character.GetTraceTargetDistanceXZ() > 3f)
        {
            _character.ChangeRoleState(eRoleState.RUN);
        }
        else if (_character.GetTraceTargetDistanceXZ() > 1f)
        {
            _character.ChangeRoleState(eRoleState.WALK);
        }
        else
        {
            _character.ChangeState(eState.FIGHTER_WEEK_ATTACK1);
        }
        // base.UpdateState();
    }
}