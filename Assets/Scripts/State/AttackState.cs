public class AttackState : State
{
    public AttackState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        _character.ActiveAttackColliders(false);
    }

    public override void UpdateState()
    {
    }
}