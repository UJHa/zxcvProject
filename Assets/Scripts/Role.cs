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
    WEEK_ATTACK_1,
    WEEK_ATTACK_2,
    WEEK_ATTACK_3,
    STRONG_ATTACK_1,
    STRONG_ATTACK_2,
    STRONG_ATTACK_3,
    AIR_WEEK_ATTACK_1,
    AIR_WEEK_ATTACK_2,
    AIR_WEEK_ATTACK_3,
    NORMAL_DAMAGED,
    DAMAGED_LANDING,
    AIRBORNE_DAMAGED,
    AIRBORNE_POWER_DOWN_DAMAGED,
    DAMAGED_AIRBORNE_LOOP,
    KNOCK_BACK_DAMAGED,
    FLY_AWAY_DAMAGED,
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