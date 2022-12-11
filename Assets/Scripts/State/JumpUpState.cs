using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpUpState : State
{
    private float _jumpTimer = 0f;
    public JumpUpState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpup start");
        // 엄todo : 이전 State 따라서 Jump CrossFadeSec 값이 다르게 주자!
        animator.CrossFade("Jump", character._jumpUpCrossFadeSec);
        character._isGround = false;
    }

    public override void UpdateState()
    {
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
        character.GetRigidbody().velocity = new Vector3(0f, character.GetJumpUpVelocity(_jumpTimer), 0f) ; 
        Debug.Log($"[jumpup]timer({_jumpTimer}) GetVelocity({character.GetJumpUpVelocity(_jumpTimer)}), position({character.transform.position}), rigid pos({character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpup end");
    }
}