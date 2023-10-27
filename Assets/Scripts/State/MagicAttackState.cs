using UnityEngine;
using Utils;

public class MagicAttackState : AttackState
{
    private bool _isAttacked = false;
    private float _attackRatio = 0.2f;
    public MagicAttackState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);
        _character.RotateToPosition(_character.GetTraceTarget().transform.position);
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
            if (_moveSet.IsAnimationFinish())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
            }

            if (!_isAttacked && _moveSet.GetCurNormTime() > _attackRatio)
            {
                _isAttacked = true;
                _character.SpawnAttackCube();
                Debug.Log($"[testMagic]Attack! {_moveSet.GetCurNormTime()}");
            }
            // bool collisionEnable = _moveSet.IsCollisionEnable(_attackInfoData);
            // _character.ActiveAttackCollider(collisionEnable, UmUtil.StringToEnum<HitboxType>(_attackInfoData.hitboxType), _attackInfoData);
        }
    }
}