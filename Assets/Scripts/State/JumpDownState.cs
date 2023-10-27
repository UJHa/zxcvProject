using UnityEngine;
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
        _moveSet.Play(_action);

        _jumpTimer = 0f;
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
            _character.ChangeRoleState(eRoleState.LANDING);
            return;
        }
        // else
        //     _character.UpdateGroundHeight();
        
        _jumpTimer += Time.fixedDeltaTime;
        // _moveVelocity.y = -1f * _character.GetJumpDownVelocity(_jumpTimer, _character.GetJumpDownMaxTimer(), _character.GetJumpMaxHeight());
        _moveVelocity.y = -1f * _character.GetJumpDownVelocity(_jumpTimer, _character.GetJumpDownMaxTimer(), _character.GetJumpMaxHeight());
        _character.SetVelocity(_moveVelocity);
    }

    public override void EndState()
    {
        
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