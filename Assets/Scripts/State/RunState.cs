using UnityEngine;
using UnityEditor;

public class RunState : State
{
    private bool _isJump = false;

    public RunState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _isJump = false;

        character.SetMoveSpeedToRun();
        animator.SetBool("Run", true);
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void EndState()
    {
        if(!_isJump)
            animator.SetBool("Run", false);
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.V) && character.IsGround())
        {
            _isJump = true;
            character.ChangeState(eState.JUMP_UP);
            return;
        }

        bool isInput = false;
        foreach (Direction direction in character.GetDirections())
        {
            if (character.GetKeysDirection(direction))
            {
                character.SetDirection(direction);

                isInput = true;
                break;
            }
        }

        if (isInput == false)
        {
            character.ChangeState(eState.IDLE);
        }
        else
        {
            character.MoveDirectionPosition(character.GetDirection());
        }
    }
}