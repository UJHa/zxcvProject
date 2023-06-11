using Animancer;
using UnityEngine;
using UnityEditor;

public class DamagedAirborneLoopState : DamagedState
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
    private AnimancerState _curState;

    public DamagedAirborneLoopState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(true, HitColliderType.AIRBORNE);
        _curState = _action.Play(1f);
        _jumpTimer = 0f;
        // _curState.Speed = 0.1f;
    }

    public override void FixedUpdateState()
    {
        _jumpTimer += Time.fixedDeltaTime;
        _moveVelocity.y = _character.GetJumpDownVelocity(_jumpTimer);
        _character.GetRigidbody().velocity = _moveVelocity;
        Debug.Log($"[damagedown]timer({_jumpTimer}) GetVelocity({_character.GetJumpUpVelocity(_jumpTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_action.IsAnimationFinish())
        {
            // _character.ChangeState(eState.DAMAGED_LANDING);
            _action.Reset();
        }
        else
        {
            var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.Z);
            if (eState.NONE != nextState2 && _eState == nextState2)
                _animancer.States.Current.Time = 0f;
        }
    }
}