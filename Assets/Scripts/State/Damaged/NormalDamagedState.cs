using UnityEngine;

public class NormalDamagedState : DamagedState
{

    public NormalDamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.SetVelocity(Vector3.zero);
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