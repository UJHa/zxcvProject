using System;
using UnityEngine;
using UnityEngine.UI;

public class BotPlayer : Character
{
    void Start()
    {
        _directionVector = Vector3.back;
        
        RegisterState(eState.FIGHTER_IDLE, typeof(NpcIdleState));
        RegisterState(eState.FIGHTER_WALK, typeof(NpcWalkState));
        RegisterState(eState.FIGHTER_RUN, typeof(NpcRunState));
        RegisterState(eState.JUMP_UP, typeof(NpcJumpUpState));
        RegisterState(eState.JUMP_DOWN, typeof(NpcJumpDownState));
        RegisterState(eState.LANDING, typeof(NpcLandingState));
        RegisterState(eState.MAGIC_WEEK_ATTACK1, typeof(MagicAttackState));
        _moveSet.Init(gameObject);
        _curState = eState.FIGHTER_IDLE;

        _stateMap[_curState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
    }
}
