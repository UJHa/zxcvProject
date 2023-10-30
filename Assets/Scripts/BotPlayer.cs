using System;
using UnityEngine;
using UnityEngine.UI;

public class BotPlayer : Character
{
    void Start()
    {
        _directionVector = Vector3.back;
        
        RegisterRoleState(eRoleState.IDLE, ActionKey.FIGHTER_IDLE, typeof(NpcIdleState));
        RegisterRoleState(eRoleState.WALK, ActionKey.FIGHTER_WALK, typeof(NpcWalkState));
        RegisterRoleState(eRoleState.RUN, ActionKey.FIGHTER_RUN, typeof(NpcRunState));
        RegisterRoleState(eRoleState.JUMP_UP, ActionKey.JUMP_UP, typeof(NpcJumpUpState));
        RegisterRoleState(eRoleState.JUMP_DOWN, ActionKey.JUMP_DOWN, typeof(NpcJumpDownState));
        RegisterRoleState(eRoleState.LANDING, ActionKey.LANDING, typeof(NpcLandingState));
        RegisterRoleState(eRoleState.WEEK_ATTACK_1, ActionKey.MAGIC_WEEK_ATTACK1, typeof(MagicAttackState));
        _moveSet.Init(gameObject);
        _curRoleState = eRoleState.IDLE;
        _roleStateMap[_curRoleState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
    }
}
