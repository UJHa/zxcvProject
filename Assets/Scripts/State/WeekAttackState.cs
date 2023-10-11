using Utils;

public class WeekAttackState : AttackState
{
    private KeyBindingType _bindingType = KeyBindingType.WEEK_ATTACK;
    
    public WeekAttackState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        _bindingType = KeyBindingType.WEEK_ATTACK;
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
            var nextState = _moveSet.DetermineNextState(_character.GetCurState(), _bindingType);
            if (eState.NONE != nextState)
                _character.ChangeState(nextState, eStateType.INPUT);
            else if (_moveSet.IsAnimationFinish())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }

            bool collisionEnable = _moveSet.IsCollisionEnable(_attackInfoData);
            _character.ActiveAttackCollider(collisionEnable, UmUtil.StringToEnum<HitboxType>(_attackInfoData.hitboxType), _attackInfoData);
        }
    }
}