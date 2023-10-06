using Animancer;
using UnityEngine;

public enum eState
{
    NONE,
    IDLE,
    WALK,
    RUN,
    RUN_STOP,
    JUMP_UP,
    JUMP_DOWN,
    LANDING,
    ATTACK,
    ATTACK2,
    ATTACK3,
    ATTACK4,
    ATTACK5,
    NORMAL_DAMAGED,
    AIRBORNE_DAMAGED,
    AIRBORNE_POWER_DOWN_DAMAGED,
    DAMAGED_AIRBORNE_LOOP,
    DAMAGED_LANDING,
    WAKE_UP,
    DEAD,
    FIGHTER_AIR_ATTACK1,
    FIGHTER_AIR_ATTACK2,
    FIGHTER_AIR_ATTACK3
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
        _action = _moveSet.GetAction(_eState);
    }

    public virtual void StartState()
    {
        _character.ActiveAttackColliders(false);
        _character.ActiveHitCollider(true, HitColliderType.STAND);
    }
    public abstract void UpdateState();
    public abstract void FixedUpdateState();

    public abstract void EndState();
}