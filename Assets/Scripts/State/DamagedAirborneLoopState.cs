using Animancer;
using UnityEngine;
using UnityEditor;

public class DamagedAirborneLoopState : DamagedState
{
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
        // _curState.Speed = 0.1f;
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        if (_action.IsAnimationFinish())
        {
            // _character.ChangeState(eState.DAMAGED_LANDING);
            _animancer.States.Current.Time = 0f;
        }
        else
        {
            var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.Z);
            if (eState.NONE != nextState2 && _eState == nextState2)
                _animancer.States.Current.Time = 0f;
        }
    }
}