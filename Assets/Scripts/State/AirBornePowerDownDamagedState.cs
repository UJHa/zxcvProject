using System;
using Animancer;
using UnityEngine;
using UnityEditor;

public class AirBornePowerDownDamagedState : DamagedState
{
    private Vector3 _moveVelocity = Vector3.zero;
    private AnimancerState _curState;
    private float _maxUpHeight = 0f;

    public AirBornePowerDownDamagedState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _curState = _action.Play();
        _character._isGround = false;
    }

    public override void FixedUpdateState()
    {
        _moveVelocity.y = -15f;
        _character.GetRigidbody().velocity = _moveVelocity;
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
        if (false == _action.IsAnimationFinish())
        {
            var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.Z);
            if (eState.NONE != nextState)
            {
                _animancer.States.Current.Time = 0f;
            }
        }
    }
}