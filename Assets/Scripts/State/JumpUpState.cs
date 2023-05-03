using UnityEngine;
using UnityEditor;
using System.Diagnostics;
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
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpup start");
        // 엄todo : 이전 State 따라서 Jump CrossFadeSec 값이 다르게 주자!
        _animator.CrossFade("Jump", _character.jumpUpStart);
        _character._isGround = false;
        _moveVelocity = Vector3.zero;
        Debug.Log($"[testum]speed({_character.GetMoveSpeed()})");
    }

    public override void UpdateState()
    {
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