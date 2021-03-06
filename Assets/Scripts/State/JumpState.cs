using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class JumpState : State
{
    private Stopwatch stopwatch;
    private long delayMillisec = 200;
    private Direction _jumpDirection = Direction.FRONT;

    public JumpState(Character character) : base(character)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        animator.SetBool("IsGround", false);
        character.StartJump();

        stopwatch.Reset();
        stopwatch.Start();

        _jumpDirection = character.GetDirection();

        character.GetComponent<Rigidbody>().AddForce(character.GetJumpForce(), ForceMode.VelocityChange);
    }

    public override void EndState()
    {
        animator.SetBool("IsGround", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        stopwatch.Stop();

        character.ResetPrevMoveSpeed();
    }

    public override void UpdateState()
    {
        if (delayMillisec <= stopwatch.ElapsedMilliseconds)
        {
            character.CheckIsGround();
            stopwatch.Stop();
        }

        if (character.IsGround())
        {
            character.ChangeState(eState.IDLE);
        }
        character.MoveDirectionPosition(_jumpDirection);

        bool isInput = false;
        foreach (Direction direction in character.GetDirections())
        {
            if (character.GetKeysDirection(direction))
            {
                character.SetDirection(direction);
                break;
            }
        }
    }
}