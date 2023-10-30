using UnityEngine;

public class DamagedState : State
{
    public DamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
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
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            if (_moveSet.IsAnimationFinish())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }
        }
    }
}