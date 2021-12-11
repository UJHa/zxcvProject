using UnityEngine;
using UnityEditor;

public enum eState
{
    IDLE,
    WALK,
    RUN,
    JUMP,
    ATTACK
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
    public abstract void EndState();
}