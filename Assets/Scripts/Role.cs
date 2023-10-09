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
}

public enum eRole
{
    FIGHTER,
    RAPIER
}

public class Role
{
    public Dictionary<eRoleState, eState> States = new();
}