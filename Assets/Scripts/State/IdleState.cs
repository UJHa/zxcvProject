using System.Diagnostics;
using Animancer;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class IdleState : State
{
    private Stopwatch _inputTimer;
    private long _inputDelayMSec = 150;
    public IdleState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        base.StartState();
        _character.ResetMoveSpeed();
        _character._isGround = true;
        var prevState = _animancer.Layers[0].CurrentState;
        if (null != prevState && prevState.Clip.Equals(_action.GetClip()))
        {
            // 재생중이던 clip이 idle clip과 동일하지 않을 때
        }
        else
        {
            // 재생중이던 clip이 idle clip과 동일할 때
            _action.Play(_character.idleStart);
        }
        
        _inputTimer.Start();

        // Idle도중 움직임이 없으므로 UpdateGroundHeight는 시작 시점 한 번만 처리
        _character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            Debug.Log("[testumAir]is not Ground!");
            _character.ChangeState(eState.JUMP_DOWN);
        }
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
                var nextState = _moveSet.DetermineNextState(_character.GetCurState(), bindingType);
                if (eState.NONE != nextState)
                {
                    _character.ChangeState(nextState, eStateType.INPUT);
                    break;
                }
            }
        }
    }
    
    private void UpdateMoveInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero != vector)
        {
            if (eState.WALK == _character.GetPrevState()
                && vector == _character.GetDirectionVector()
                && _inputTimer.ElapsedMilliseconds <= _inputDelayMSec)
            {
                _character.ChangeState(eState.RUN);
            }
            else
            {
                _character.ChangeState(eState.WALK);
            }
        }
    }
}