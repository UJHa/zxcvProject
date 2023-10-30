using DataClass;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NpcJumpUpState : JumpUpState
{
    private float _jumpTimer = 0f;
    private Vector3 _moveVelocity = Vector3.zero;

    public NpcJumpUpState(Character character, ActionKey actionKey) : base(character, actionKey)
    {
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        // base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void EndState()
    {
        base.EndState();
    }
}