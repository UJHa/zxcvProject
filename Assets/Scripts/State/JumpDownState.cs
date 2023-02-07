using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpDownState : State
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;
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
        UpdateMoveXZ();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = character.GetGroundCheckObjects();
        if (groundObjs.Length > 0)
        {
            Debug.Log("[testumLanding]isGround!");
            character.ChangeState(eState.LANDING);
            return;
        }
        else
            character.UpdateGroundHeight();
        
        _jumpTimer += Time.fixedDeltaTime;
        _moveVelocity.y = character.GetJumpDownVelocity(_jumpTimer);
        character.GetRigidbody().velocity = _moveVelocity;
        // character.GetRigidbody().velocity = new Vector3(0f, character.GetJumpDownVelocity(_jumpTimer), 0f) ; 
        // Debug.Log($"[jumpdown]timer({_jumpTimer}) GetVelocity({character.GetJumpDownVelocity(_jumpTimer)}), position({character.transform.position}), rigid pos({character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpdown end");
    }
    
    void UpdateMoveXZ()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();

        if (Vector3.zero != vector)
            character.SetDirectionByVector3(vector);
        var moveVector = character.GetMoveDirectionVector(vector);
        _moveVelocity = moveVector * character.GetMoveSpeed();
    }
}