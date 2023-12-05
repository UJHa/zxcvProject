using System.Collections;
using UnityEngine;

public class GuardState : State
{
    public GuardState(Character character, ActionKey actionKey) : base(character, actionKey)
    {

    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        if (_character.GetPrevRoleState() == eRoleState.GUARD_DAMAGED)
        {
            _moveSet.SetAnimationEndRatio();
        }
        _character.SetVelocity(Vector3.zero);
        _character.SetGuard(true);
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
            _moveSet.PauseAnimation();
        }
        if (false == InputManager.Instance.GetButton(KeyBindingType.GUARD))
        {
            _character.SetGuard(false);
            _character.ChangeRoleState(eRoleState.IDLE);
            return;
        }
        //_character.ChangeRoleState(eRoleState.FIGHTER_GUARD_DAMAGED);
        //if (_moveSet.IsAnimationFinish())
        //{
        //}
    }
}