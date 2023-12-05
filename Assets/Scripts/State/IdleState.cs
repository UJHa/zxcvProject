using System.Diagnostics;
using DataClass;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class IdleState : State
{
    private Stopwatch _inputTimer = new();
    private long _inputDelayMSec = 150;
    public IdleState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
        
    }

    public override void StartState()
    {
        base.StartState();
        _character.ResetMoveSpeed();
        _character.SetGrounding(true);
        if (false == _moveSet.IsEqualClip(_action))
        {
            // 재생중이던 clip이 idle clip과 동일할 때
            AnimationFadeInfoData data = _character.GetAnimFadeInfoData();
            _moveSet.Play(_action, data.idleStart);
        }

        _character.StopRotateDirection();
        
        _inputTimer.Start();
        
        if (_character.IsGround())
            _character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        if (!_character.IsGround())
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        if (!InputManager.IsExistInstance)
            return;

        UpdateMoveInput();

        if (_character.IsGround())
        {
            var nextState = _moveSet.DetermineNextState(_character.GetCurState());
            if (eRoleState.NONE != nextState)
            {
                _character.ChangeRoleState(nextState, eStateType.INPUT);
                return;
            }
        }
    }
    
    private void UpdateMoveInput()
    {
        var vector = _character.GetMoveInputVector();
        if (Vector3.zero != vector)
        {
            if (eRoleState.WALK == _character.GetPrevRoleState()
                && vector == _character.GetDirectionVector()
                && _inputTimer.ElapsedMilliseconds <= _inputDelayMSec)
            {
                _character.ChangeRoleState(eRoleState.RUN);
            }
            else
            {
                _character.ChangeRoleState(eRoleState.WALK);
            }
        }
    }
}