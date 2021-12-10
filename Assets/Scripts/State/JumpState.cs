using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class JumpState : State
{
    private Stopwatch stopwatch;
    private long delayMillisec = 200;
    private Direction _jumpDirection = Direction.FRONT;

    public JumpState(Player player) : base(player)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        eState prevState = player.GetPrevState();
        if (prevState == eState.IDLE)
            player.ResetMoveSpeed();
        if (prevState == eState.WALK)
            player.SetWalkSpeed();
        if (prevState == eState.RUN)
            player.SetRunSpeed();

        animator.SetBool("IsGround", false);
        player.StartJump();

        stopwatch.Reset();
        stopwatch.Start();

        _jumpDirection = player.GetDirection();

        player.GetComponent<Rigidbody>().AddForce(player.GetJumpForce(), ForceMode.VelocityChange);
    }

    public override void EndState()
    {
        animator.SetBool("IsGround", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        stopwatch.Stop();

        player.ResetPrevMoveSpeed();
    }

    public override void UpdateState()
    {
        if (delayMillisec <= stopwatch.ElapsedMilliseconds)
        {
            player.CheckIsGround();
            stopwatch.Stop();
        }

        if (player.IsGround())
        {
            player.ChangeState(eState.IDLE);
        }
        player.MoveDirectionPosition(_jumpDirection);

        bool isInput = false;
        foreach (Direction direction in player.GetDirections())
        {
            if (player.GetKeysDirection(direction))
            {
                player.SetDirection(direction);
                break;
            }
        }
    }
}