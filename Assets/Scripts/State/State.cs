using UnityEngine;
using UnityEditor;

public enum eState
{
    IDLE,
    WALK,
    RUN,
    JUMP
}

public abstract class State
{
    protected Player player;
    protected Animator animator;

    public State(Player player)
    {
        this.player = player;
        this.animator = player.GetComponent<Animator>();
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void EndState();
}