using Utils;

public class MeleeAttackState : AttackState
{
    public MeleeAttackState(Character character, ActionKey actionKey) : base(character, actionKey)
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
        if (_character.IsGround())
        {
            var nextState = _moveSet.DetermineNextState(_character.GetCurState());
            if (eRoleState.NONE != nextState)
                _character.ChangeRoleState(nextState, eStateType.INPUT);
            else if (_moveSet.IsAnimationFinish())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }

            bool collisionEnable = _moveSet.IsCollisionEnable(_attackInfoData);
            _character.ActiveAttackCollider(collisionEnable, UmUtil.StringToEnum<HitboxType>(_attackInfoData.hitboxType), _attackInfoData);
        }
    }
}