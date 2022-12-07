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
        animator.Play("Jump");
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpdown start");
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (character.IsGroundCheck())
        {
            Debug.Log("[testumLanding]isGround!");
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