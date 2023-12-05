using System.Collections;
using UnityEngine;

public class GuardDamagedState : State
{
    public GuardDamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
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

    }

    public override void UpdateState()
    {
        if (_moveSet.IsAnimationFinish())
        {
            if (false == InputManager.Instance.GetButton(KeyBindingType.GUARD))
            {
                _character.SetGuard(false);
                _character.ChangeRoleState(eRoleState.IDLE);
                return;
            }
            else
            {
                _character.ChangeRoleState(eRoleState.GUARD);
            }
        }
    }
}