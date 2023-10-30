using DataClass;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class JumpUpState : State
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public JumpUpState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _jumpTimer = 0f;
        AnimationFadeInfoData data = _character.GetAnimFadeInfoData();
        _moveSet.Play(_action, data.jumpUpStart);
        _character._isGround = false;
        _moveVelocity = Vector3.zero;
        _character.SetVelocity(_moveVelocity);
    }

    public override void UpdateState()
    {
        if (_moveSet.IsAnimationFinish())
            _moveSet.SetAnimationEndRatio();
        var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyBindingType.WEEK_ATTACK);
        if (eRoleState.NONE != nextState)
            _character.ChangeState(nextState, eStateType.INPUT);
        UpdateMoveXZ();
    }

    public override void FixedUpdateState()
    {
        if (_jumpTimer >= _character.GetJumpUpMaxTimer())
        {
            // Debug.Log($"[testlog] jump up update fin?");
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
            return;
        }
        _jumpTimer += Time.fixedDeltaTime;
        // _moveVelocity.y = _character.GetJumpUpVelocity(_jumpTimer, _character.GetJumpUpMaxTimer(), _character.GetJumpMaxHeight());
        _moveVelocity.y = _character.GetJumpUpVelocity(_jumpTimer, _character.GetJumpUpMaxTimer(), _character.GetJumpMaxHeight());
        _character.SetVelocity(_moveVelocity);
        // Debug.Log($"[jumpup]timer({_jumpTimer}) GetVelocity({_character.GetJumpUpVelocity(_jumpTimer)}), position({_character.transform.position}), rigid pos({_character.GetRigidbody().position})");
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