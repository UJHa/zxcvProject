using UnityEngine;

public enum eState
{
    IDLE,
    WALK,
    RUN,
    RUNSTOP,
    JUMP_UP,
    JUMP_DOWN,
    LANDING,
    ATTACK,
    DEAD
}

public abstract class State
{
    protected Character character;
    protected Animator animator;

    public State(Character character)
    {
        this.character = character;
        this.animator = character.GetComponent<Animator>();
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void EndState();
}