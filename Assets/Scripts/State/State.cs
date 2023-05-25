using Animancer;
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
    ATTACK4,
    ATTACK5,
    DAMAGED,
    DEAD
}

public abstract class State
{
    protected Character _character;
    protected readonly AnimancerComponent _animancer;
    protected readonly eState _eState;
    protected readonly MoveSet _moveSet;
    protected readonly Action _action;

    public State(Character character, eState eState)
    {
        this._character = character;
        this._animancer = _character.GetComponent<AnimancerComponent>();
        _moveSet = _character.GetMoveSet();
        _eState = eState;
        _action = _moveSet.GetCurAction(_eState);
    }

    public virtual void StartState()
    {
        _character.ActiveAttackColliders(false);
    }
    public abstract void UpdateState();
    public abstract void FixedUpdateState();

    public abstract void EndState();
}