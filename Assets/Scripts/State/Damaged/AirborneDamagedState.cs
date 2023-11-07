using System;
using UnityEngine;
using UnityEditor;

public class AirborneDamagedState : DamagedState
{
    private float _upMoveTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
    private float _maxUpHeight = 0f;

    public AirborneDamagedState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _moveSet.Play(_action);
        _maxUpHeight = _character.transform.position.y + _character.GetAttackedMaxHeight();
        Debug.Log($"[testum][{_character.name}]_maxUpHeight({_maxUpHeight})");
        _upMoveTimer = 0f;
        _character.SetGrounding(false);
    }

    public override void FixedUpdateState()
    {
        if (_character.transform.position.y >= _maxUpHeight)
        {
            Debug.Log($"[testlog] damage up update fin?");
            _character.ChangeRoleState(eRoleState.DAMAGED_AIRBORNE_LOOP);
        }
        else
        {
            _upMoveTimer += Time.fixedDeltaTime;
            _moveVelocity.y = _character.GetAirBoneUpVelocity(_upMoveTimer, _character.GetAttackedAirborneUpTime(), _character.GetAttackedMaxHeight());
            _character.SetVelocity(_moveVelocity);
        }
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
    }
}