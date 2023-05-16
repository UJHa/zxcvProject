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
    private AnimationClip _animClip = null;
    public IdleState(Character character, eState eState) : base(character, eState)
    {
        _inputTimer = new Stopwatch();
    }

    public override void StartState()
    {
        _character.ResetMoveSpeed();
        _character._isGround = true;
        if (null == _animClip)
            _animClip = Resources.Load<AnimationClip>("Animation/Idle");
        _animancer.Play(_animClip, _character.idleStart);
        
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
        UpdateAnimation();
        UpdateInput();
    }

    private void UpdateAnimation()
    {
        // var curState = _animancer.States.Current;
        // Debug.Log($"[testum]TTT curState({curState.Key})");
        // if (curState.Key.Equals("JumpEnd") && curState.NormalizedTime >= 1.0f)
        //     _animancer.Play(_animClip, _character.idleStart);
    }

    private void UpdateInput()
    {
        if (!InputManager.IsExistInstance)
            return;
        
        UpdateMoveInput();

        if (_character.IsGround())
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                _character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
            }
            // else if (Input.GetKeyDown(KeyCode.C))
            else
            {
                var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.C);
                Debug.Log($"[testum]idleChange Test ({nextState})");
                if (eState.NONE != nextState)
                    _character.ChangeState(nextState, eStateType.INPUT);
                else
                {
                    var nextState2 = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.X);
                    if (eState.NONE != nextState2)
                        _character.ChangeState(nextState2, eStateType.INPUT);
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