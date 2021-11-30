using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class JumpState : State
{
    private Stopwatch stopwatch;
    private long delayMillisec = 20;

    public JumpState(Player player) : base(player)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        animator.SetBool("IsGround", false);
        player.StartJump();

        stopwatch.Reset();
        stopwatch.Start();

        player.GetComponent<Rigidbody>().AddForce(player.GetJumpForce());
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
    }
}