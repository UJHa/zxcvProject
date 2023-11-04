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
        _character._isGround = true;
        if (false == _moveSet.IsEqualClip(_action))
        {
            // 재생중이던 clip이 idle clip과 동일할 때
            AnimationFadeInfoData data = _character.GetAnimFadeInfoData();
            _moveSet.Play(_action, data.idleStart);
        }
        
        _inputTimer.Start();

        // Idle도중 움직임이 없으므로 UpdateGroundHeight는 시작 시점 한 번만 처리
        // 엄todo : 이 함수 기반으로 점프 바닥 충돌 위치 보간 테스트하기
        _character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.RefreshGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }
        _character.SetVelocity(Vector3.zero);
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (!InputManager.IsExistInstance)
            return;
        
        UpdateMoveInput();

        if (_character.IsGround())
        {
            KeyBindingType[] keyBindingTypes = new[]
            {
                KeyBindingType.JUMP, 
                KeyBindingType.WEEK_ATTACK, 
                KeyBindingType.STRONG_ATTACK,
                KeyBindingType.INTERACTION
            };
            foreach (var bindingType in keyBindingTypes)
            {
                
            }
            var nextState = _moveSet.DetermineNextState(_character.GetCurState());
            if (eRoleState.NONE != nextState)
            {
                _character.ChangeState(nextState, eStateType.INPUT);
            }
        }
    }
    
    private void UpdateMoveInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
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