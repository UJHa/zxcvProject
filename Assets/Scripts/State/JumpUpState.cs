using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Animancer;
using Debug = UnityEngine.Debug;

public class JumpUpState : State
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public JumpUpState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpup start");
        _action.Play(_character.jumpUpStart);
        _character._isGround = false;
        _moveVelocity = Vector3.zero;
        Debug.Log($"[testum]speed({_character.GetMoveSpeed()})");
        _character.GetRigidbody().velocity = Vector3.zero;
    }

    public override void UpdateState()
    {
        var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyBindingType.WEEK_ATTACK);
        if (eState.NONE != nextState)
            _character.ChangeState(nextState, eStateType.INPUT);
        UpdateMoveXZ();
    }

    public override void FixedUpdateState()
    {
        if (_jumpTimer >= _character.GetJumpUpMaxTimer())
        {
            Debug.Log($"[testlog] jump up update fin?");
            _character.ChangeState(eState.JUMP_DOWN);
            return;
        }
        _jumpTimer += Time.fixedDeltaTime;
        _moveVelocity.y = _character.GetJumpUpVelocity(_jumpTimer);
        _character.GetRigidbody().velocity = _moveVelocity;
        Debug.Log($"[jumpup]timer({_jumpTimer}) GetVelocity({_character.GetJumpUpVelocity(_jumpTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpup end");
    }
    
    void UpdateMoveXZ()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();

        if (Vector3.zero != vector)
            _character.SetDirectionByVector3(vector);
        var moveVector = _character.GetMoveDirectionVector(vector);
        _moveVelocity = moveVector * _character.GetMoveSpeed();
    }
}