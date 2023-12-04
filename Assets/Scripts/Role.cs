using System.Collections.Generic;

public enum eRoleState
{
    NONE,
    IDLE,
    WALK,
    RUN,
    RUN_STOP,
    JUMP_UP,
    JUMP_DOWN,
    LANDING,
    WEEK_ATTACK1,
    WEEK_ATTACK2,
    WEEK_ATTACK3,
    STRONG_ATTACK1,
    STRONG_ATTACK2,
    STRONG_ATTACK3,
    AIR_WEEK_ATTACK1,
    AIR_WEEK_ATTACK2,
    AIR_WEEK_ATTACK3,
    NORMAL_DAMAGED,
    DAMAGED_LANDING,
    AIRBORNE_DAMAGED,
    AIRBORNE_POWER_DOWN_DAMAGED,
    DAMAGED_AIRBORNE_LOOP,
    KNOCK_BACK_DAMAGED,
    FLY_AWAY_DAMAGED,
    RUN_ATTACK,
    WAKE_UP,
    DEAD,
    GET_ITEM
}

public enum eRole
{
    FIGHTER,
    RAPIER
}

public class Role
{
    public Dictionary<eRoleState, ActionKey> States = new();
}