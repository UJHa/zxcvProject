public class AttackState : State
{
    public AttackState(Character character, eState eState) : base(character, eState)
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