using UnityEngine;
using UnityEditor;

public enum eState
{
    IDLE,
    WALK,
    RUN,
    JUMP,
    JUMP_UP,
    JUMP_DOWN,
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