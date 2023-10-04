using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Animancer;
using Debug = UnityEngine.Debug;

public class JumpDownState : State
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public JumpDownState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _action.Play();
        
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpdown start");
    }

    public override void UpdateState()
    {
        UpdateMoveXZ();
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (groundObjs.Length > 0)
        {
            Debug.Log("[testumLanding]isGround!");
            _character.ChangeState(eState.LANDING);
            return;
        }
        else
            _character.UpdateGroundHeight();
        
        _jumpTimer += Time.fixedDeltaTime;
        _moveVelocity.y = _character.GetJumpDownVelocity(_jumpTimer);
        _character.GetRigidbody().velocity = _moveVelocity;
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpdown end");
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