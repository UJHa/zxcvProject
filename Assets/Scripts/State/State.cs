using UnityEngine;

public enum eState
{
    NONE,
    IDLE,
    WALK,
    RUN,
    RUNSTOP,
    JUMP_UP,
    JUMP_DOWN,
    LANDING,
    ATTACK,
    ATTACK2,
    ATTACK3,
    DEAD
}

public abstract class State
{
    protected Character _character;
    protected Animator _animator;
    protected readonly eState _eState;

    public State(Character character, eState eState)
    {
        this._character = character;
        this._animator = character.GetComponent<Animator>();
        _eState = eState;
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void EndState();
}