using System;
using Animancer;
using UnityEngine;
using UnityEditor;

public enum AirborneState
{
    UP,
    DOWN,
}
public class AirborneDamagedState : DamagedState
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
    private AnimancerState _curState;
    private AirborneState _airborneState = AirborneState.UP;

    public AirborneDamagedState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _curState = _action.Play();
        _jumpTimer = 0f;
        _airborneState = AirborneState.UP;
    }

    public override void FixedUpdateState()
    {
        switch (_airborneState)
        {
            case AirborneState.UP:
                if (_jumpTimer >= _character.GetJumpUpMaxTimer())
                {
                    Debug.Log($"[testlog] damage up update fin?");
                    _jumpTimer = 0f;
                    _airborneState = AirborneState.DOWN;
                }
                else
                {
                    _jumpTimer += Time.fixedDeltaTime;
                    _moveVelocity.y = _character.GetJumpUpVelocity(_jumpTimer);
                    _character.GetRigidbody().velocity = _moveVelocity;
                }
                Debug.Log($"[damageup]timer({_jumpTimer}) GetVelocity({_character.GetJumpUpVelocity(_jumpTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
                break;
            case AirborneState.DOWN:
                if (_action.IsAnimationFinish())
                {
                    _character.ChangeState(eState.DAMAGED_AIRBORNE_LOOP);
                }
                else
                {
                    _jumpTimer += Time.fixedDeltaTime;
                    _moveVelocity.y = _character.GetJumpDownVelocity(_jumpTimer);
                    _character.GetRigidbody().velocity = _moveVelocity;
                }
                Debug.Log($"[damagedown]timer({_jumpTimer}) GetVelocity({_character.GetJumpUpVelocity(_jumpTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
                break;
        }
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