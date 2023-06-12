using System;
using Animancer;
using UnityEngine;
using UnityEditor;

public class AirborneDamagedState : DamagedState
{
    private float _upMoveTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
    private AnimancerState _curState;
    private float _maxUpHeight = 0f;

    public AirborneDamagedState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _curState = _action.Play();
        _maxUpHeight = _character.transform.position.y + _character.GetAttackedMaxHeight();
        Debug.Log($"[testum]_maxUpHeight({_maxUpHeight})");
        _upMoveTimer = 0f;
    }

    public override void FixedUpdateState()
    {
        if (_character.transform.position.y >= _maxUpHeight)
        {
            Debug.Log($"[testlog] damage up update fin?");
            _character.ChangeState(eState.DAMAGED_AIRBORNE_LOOP);
        }
        else
        {
            _upMoveTimer += Time.fixedDeltaTime;
            _moveVelocity.y = _character.GetDamagedUpVelocity(_upMoveTimer, 1f);
            _character.GetRigidbody().velocity = _moveVelocity;
        }
        Debug.Log($"[damageup]timer({_upMoveTimer}) GetVelocity({_character.GetJumpUpVelocity(_upMoveTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_action.IsAnimationFinish())
        {
            // _character.ChangeState(eState.DAMAGED_AIRBORNE_LOOP);
        }
        else
        {
            var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.Z);
            if (eState.NONE != nextState2)// && _eState == nextState2)
            {
                _animancer.States.Current.Time = 0f;
            }
        }
    }
}