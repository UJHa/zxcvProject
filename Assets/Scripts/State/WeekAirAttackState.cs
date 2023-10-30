using UnityEngine;
using Utils;

public class WeekAirAttackState : AttackState
{
    public WeekAirAttackState(Character character, ActionKey actionKey) : base(character, actionKey)
    {

    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        _character.SetVelocity(Vector3.zero);
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {

    }

    public override void UpdateState()
    {
        var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyBindingType.WEEK_ATTACK);
        if (eRoleState.NONE != nextState)
            _character.ChangeState(nextState, eStateType.INPUT);
        else if (_moveSet.IsAnimationFinish())
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }

        bool collisionEnable = _moveSet.IsCollisionEnable(_attackInfoData);
        _character.ActiveAttackCollider(collisionEnable, UmUtil.StringToEnum<HitboxType>(_attackInfoData.hitboxType), _attackInfoData);
    }
}