using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpUpState : State
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
    public JumpUpState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpup start");
        // 엄todo : 이전 State 따라서 Jump CrossFadeSec 값이 다르게 주자!
        animator.CrossFade("Jump", character.jumpUpStart);
        character._isGround = false;
        _moveVelocity = Vector3.zero;
        Debug.Log($"[testum]speed({character.GetMoveSpeed()})");
    }

    public override void UpdateState()
    {
        UpdateMoveXZ();
    }

    public override void FixedUpdateState()
    {
        if (_jumpTimer >= character.GetJumpUpMaxTimer())
        {
            Debug.Log($"[testlog] jump up update fin?");
            character.ChangeState(eState.JUMP_DOWN);
            return;
        }
        _jumpTimer += Time.fixedDeltaTime;
        _moveVelocity.y = character.GetJumpUpVelocity(_jumpTimer);
        character.GetRigidbody().velocity = _moveVelocity;
        Debug.Log($"[jumpup]timer({_jumpTimer}) GetVelocity({character.GetJumpUpVelocity(_jumpTimer)}), position({character.transform.position}), rigid pos({character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpup end");
    }
    
    void UpdateMoveXZ()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();

        if (Vector3.zero != vector)
            character.SetDirectionByVector3(vector);
        _moveVelocity = vector * character.GetMoveSpeed();
    }
}