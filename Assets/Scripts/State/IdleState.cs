using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;

public class IdleState : State
{
    public IdleState(Character character) : base(character)
    {

    }

    public override void StartState()
    {
        character.ResetMoveSpeed();
        character._isGround = true;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            animator.Play("JumpEnd");
        else
            animator.Play("Idle");
    }

    public override void FixedUpdateState()
    {
        if (false == character.IsGroundCheck())
        {
            Debug.Log("[testumAir]is not Ground!");
            character.ChangeState(eState.JUMP_DOWN);
        }
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
        UpdateAnimation();
        foreach (Direction direction in character.GetDirections())
        {
            if (character.GetKeysDownDirection(direction))
            {
                character.SetDirection(direction);

                character.ChangeState(eState.WALK);
            }
        }

        if(Input.GetKeyDown(KeyCode.V) && character.IsGround())
        {
            character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
        }

        if (Input.GetKeyDown(KeyCode.C) && character.IsGround())
        {
            character.ChangeState(eState.ATTACK);
        }
    }

    private void UpdateAnimation()
    {
        var curStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (curStateInfo.IsName("JumpEnd"))
            if (curStateInfo.normalizedTime >= 1.0f)
                animator.Play("Idle");
    }
}