public class NormalDamagedState : DamagedState
{

    public NormalDamagedState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }
}