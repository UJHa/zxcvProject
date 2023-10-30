using DataClass;

public abstract class State
{
    protected Character _character;
    protected ActionKey _actionKey;
    protected readonly MoveSet _moveSet;
    protected Action _action;
    protected AttackInfoData _attackInfoData;

    public State(Character character, ActionKey actionKey)
    {
        this._character = character;
        _moveSet = _character.GetMoveSet();
        _actionKey = actionKey;
    }

    public virtual void StartState()
    {
        _character.ActiveAttackColliders(false);
        _character.ActiveHitCollider(true, HitColliderType.STAND);
        _action = GameManager.Instance.GetAction(_actionKey);
        _attackInfoData = AttackInfoTable.GetData(_actionKey.ToString());
    }
    public abstract void UpdateState();
    public abstract void FixedUpdateState();

    public abstract void EndState();

    public void SetState(ActionKey state)
    {
        _actionKey = state;
    }
}