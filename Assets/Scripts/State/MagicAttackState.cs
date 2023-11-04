using UnityEngine;
using Utils;

public class MagicAttackState : AttackState
{
    private bool _isAttacked = false;
    private float _attackRatio = 0.2f;
    public MagicAttackState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        // bot 개발 시 분리된 상속 클래스에서 처리
        // _character.RotateToPosition(_character.GetTraceTarget().transform.position);
        _isAttacked = false;
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
                _character.ChangeState(nextState, eStateType.INPUT);
            if (_moveSet.IsAnimationFinish())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }

            if (!_isAttacked && _moveSet.GetCurNormTime() > _attackRatio)
            {
                _isAttacked = true;
                var projectile = _character.SpawnAttackCube(_actionKey);
                Debug.Log($"[testMagic]Attack! {_moveSet.GetCurNormTime()}");
                projectile.SetAttackInfo(_attackInfoData);
            }
            // bool collisionEnable = _moveSet.IsCollisionEnable(_attackInfoData);
            // _character.ActiveAttackCollider(collisionEnable, UmUtil.StringToEnum<HitboxType>(_attackInfoData.hitboxType), _attackInfoData);
        }
    }
}