using Animancer;
using UnityEngine;
using UnityEditor;

public class DamagedLandingState : DamagedState
{
    private AnimancerState _curState;

    public DamagedLandingState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ActiveHitCollider(false, HitColliderType.STAND);
        _character.ActiveHitCollider(false, HitColliderType.AIRBORNE);
        _curState = _action.Play(0.3f);
        _character.GetRigidbody().velocity = Vector3.zero;
        _character.UpdateGroundHeight(true);
        _character._isGround = true;
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.WAKE_UP);
            }
            else
            {
                var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.Z);
                if (eState.NONE != nextState2 && _eState == nextState2)
                    _animancer.States.Current.Time = 0f;
            }
        }
    }
}