using UnityEngine;
using UnityEditor;

public enum eState
{
    IDLE,
    RUN,
    JUMP
}

public abstract class State
{
    protected Player player;

    public State(Player player)
    {
        this.player = player;
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void EndState();
}