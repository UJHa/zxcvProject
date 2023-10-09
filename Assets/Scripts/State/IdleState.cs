using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class IdleState : State
{
    private Stopwatch _inputTimer = new();
    private long _inputDelayMSec = 150;
    public IdleState(Character character, eState eState) : base(character, eState)
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
            _moveSet.Play(_action, _character.idleStart);
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
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
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