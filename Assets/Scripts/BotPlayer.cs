using System;
using UnityEngine;
using UnityEngine.UI;

public class BotPlayer : Character
{
    void Start()
    {
        _directionVector = Vector3.back;
        
        RegisterRoleState(eRoleState.IDLE, eState.FIGHTER_IDLE, typeof(NpcIdleState));
        RegisterRoleState(eRoleState.WALK, eState.FIGHTER_WALK, typeof(NpcWalkState));
        RegisterRoleState(eRoleState.RUN, eState.FIGHTER_RUN, typeof(NpcRunState));
        RegisterRoleState(eRoleState.JUMP_UP, eState.JUMP_UP, typeof(NpcJumpUpState));
        RegisterRoleState(eRoleState.JUMP_DOWN, eState.JUMP_DOWN, typeof(NpcJumpDownState));
        RegisterRoleState(eRoleState.LANDING, eState.LANDING, typeof(NpcLandingState));
        RegisterRoleState(eRoleState.WEEK_ATTACK_1, eState.MAGIC_WEEK_ATTACK1, typeof(MagicAttackState));
        _moveSet.Init(gameObject);
        _curRoleState = eRoleState.IDLE;
        _roleStateMap[_curRoleState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
    }
}
