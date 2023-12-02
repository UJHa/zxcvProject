using System;
using UnityEngine;
using UnityEngine.UI;

public class BotPlayer : Character
{
    void Start()
    {
        _directionVector = Vector3.back;
        
        RegisterRoleState(eRoleState.IDLE, ActionKey.FIGHTER_IDLE, typeof(IdleState));
        RegisterRoleState(eRoleState.WALK, ActionKey.FIGHTER_IDLE, typeof(IdleState));
        RegisterRoleState(eRoleState.RUN, ActionKey.FIGHTER_IDLE, typeof(IdleState));
        //RegisterRoleState(eRoleState.WEEK_ATTACK1, ActionKey.MAGIC_WEEK_ATTACK1, typeof(MagicAttackState));
        _moveSet.Init(gameObject);
        _curRoleState = eRoleState.IDLE;
        _roleStateMap[_curRoleState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
    }
}
