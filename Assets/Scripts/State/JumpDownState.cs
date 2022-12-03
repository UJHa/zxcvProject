using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpDownState : State
{
    private float _jumpTimer = 0f;
    public JumpDownState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpdown start");
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (_jumpTimer >= character.GetJumpUpMaxTimer())
        {
            // 엄todo : 무한히 떨어지고 바닥 체크하도록 변경!!
            character.ChangeState(eState.IDLE);
            return;
        }
        _jumpTimer += Time.fixedDeltaTime;
        character.GetRigidbody().velocity = new Vector3(0f, character.GetJumpDownVelocity(_jumpTimer), 0f) ; 
        Debug.Log($"[jumpdown]timer({_jumpTimer}) GetVelocity({character.GetJumpDownVelocity(_jumpTimer)}), position({character.transform.position}), rigid pos({character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpdown end");
    }
}