using DataClass;
using UnityEngine;

public class WalkState : State
{
    public WalkState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.SetMoveSpeedToWalk();
        AnimationFadeInfoData data = _character.GetAnimFadeInfoData();
        _moveSet.Play(_action, data.walkStart);
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.RefreshGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            _character.ChangeRoleState(eRoleState.JUMP_DOWN);
        }
        else
            _character.UpdateGroundHeight();
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
        UpdateInput();
    }
    
    void UpdateInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero == vector)
        {
            _character.ChangeRoleState(eRoleState.IDLE);
            return;
        }

        if (InputManager.Instance.GetButtonDown(KeyBindingType.JUMP))
        {
            _character.ChangeRoleState(eRoleState.JUMP_UP, eStateType.INPUT);
            return;
        }
        
        _character.SetDirectionByVector3(vector);
        _character.MovePosition(vector);
    }
}