using System.Diagnostics;
using UnityEngine;

public class LandingState : State
{
    private Stopwatch _inputTimer;
    private long _inputDelayMSec = 150;

    public LandingState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        base.StartState();
        _character.ResetMoveSpeed();
        _character._isGround = true;
        _moveSet.Play(_action, _character.jumpEnd);
        _inputTimer.Start();

        _character.UpdateGroundHeight();
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void EndState()
    {
        _inputTimer.Reset();
    }

    public override void UpdateState()
    {
        UpdateAnimation();
        UpdateInput();
    }

    private void UpdateAnimation()
    {
        if (_moveSet.IsAnimationFinish())
            _character.ChangeRoleState(eRoleState.IDLE);
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
            // if(Input.GetKeyDown(InputManager.Instance.GetKeyCode(KeyBindingType.JUMP)))
            // {
            //     _character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
            // }
            //
            // if (Input.GetKeyDown(InputManager.Instance.GetKeyCode(KeyBindingType.WEEK_ATTACK)))
            // {
            //     _character.ChangeState(eState.ATTACK);
            // }
        }
    }
    
    private void UpdateMoveInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero != vector)
        {
            _character.ChangeRoleState(eRoleState.WALK);
        }
    }
}