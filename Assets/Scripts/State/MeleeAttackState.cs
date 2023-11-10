using UnityEngine;

public class MeleeAttackState : AttackState
{
    Vector3 _offset = Vector3.zero;
    Vector3 _size = Vector3.zero;
    public MeleeAttackState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _moveSet.Play(_action);

        _offset = new(_attackInfoData.offset[0], _attackInfoData.offset[1], _attackInfoData.offset[2]);
        _size = new(_attackInfoData.size[0], _attackInfoData.size[1], _attackInfoData.size[2]);
    }

    public override void FixedUpdateState()
    {
    }

    public override void DrawGizmosUpdateState()
    {
        base.DrawGizmosUpdateState();
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS( _character.transform.position, _character.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero + _offset, _size);
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
            if (collisionEnable)
            {
                 RaycastHit[] hits = _character.HitBoxCast(_offset, _size);
                 foreach (var hit in hits)
                 {
                     if (false == hit.collider.TryGetComponent<HitCollider>(out var hitCollider))
                         continue;
                         
                     var instanceID = hitCollider.GetCharacter().GetInstanceID();
                     if (_instanceIds.Contains(instanceID))
                         continue;
                     _instanceIds.Add(instanceID);
                     
                     // OnHit Process
                     HitInfo hitInfo = new()
                     {
                         Attacker = _character,
                         AttackInfoData = _attackInfoData,
                         RaycastHit = hit
                     };
                     hitCollider.OnHit(hitInfo);
                 }
            }
        }
    }
}