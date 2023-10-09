public enum eState
{
    NONE,
    FIGHTER_IDLE,
    FIGHTER_WALK,
    FIGHTER_RUN,
    FIGHTER_RUN_STOP,
    JUMP_UP,
    JUMP_DOWN,
    LANDING,
    FIGHTER_WEEK_ATTACK1,
    FIGHTER_WEEK_ATTACK2,
    FIGHTER_WEEK_ATTACK3,
    FIGHTER_STRONG_ATTACK1,
    FIGHTER_STRONG_ATTACK2,
    FIGHTER_WEEK_AIR_ATTACK1,
    FIGHTER_WEEK_AIR_ATTACK2,
    FIGHTER_WEEK_AIR_ATTACK3,
    NORMAL_DAMAGED,
    AIRBORNE_DAMAGED,
    AIRBORNE_POWER_DOWN_DAMAGED,
    DAMAGED_AIRBORNE_LOOP,
    DAMAGED_LANDING,
    WAKE_UP,
    DEAD,
    GET_ITEM,
    RAPIER_IDLE,
    RAPIER_WALK,
    RAPIER_RUN,
    RAPIER_RUN_STOP,
    RAPIER_JUMP_UP,
    RAPIER_JUMP_DOWN,
    RAPIER_LANDING,
}

public abstract class State
{
    protected Character _character;
    protected readonly eState _eState;
    protected readonly MoveSet _moveSet;
    protected Action _action;

    public State(Character character, eState eState)
    {
        this._character = character;
        _moveSet = _character.GetMoveSet();
        _eState = eState;
    }

    public virtual void StartState()
    {
        _character.ActiveAttackColliders(false);
        _character.ActiveHitCollider(true, HitColliderType.STAND);
        _action = GameManager.Instance.GetAction(_eState);
    }
    public abstract void UpdateState();
    public abstract void FixedUpdateState();

    public abstract void EndState();
}