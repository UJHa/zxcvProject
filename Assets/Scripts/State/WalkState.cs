using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class WalkState : State
{
    private Stopwatch stopwatch;
    private long delayMillisec = 150;
    private bool isAllKeyUp = false;

    public WalkState(Player player) : base(player)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        UnityEngine.Debug.Log("State Check : Walk Start");
        player.SetWalkSpeed();
        stopwatch.Reset();
        isAllKeyUp = false;

        animator.SetBool("Walk", true);
    }

    public override void EndState()
    {
        UnityEngine.Debug.Log("State Check : Walk End");
        stopwatch.Stop();
        animator.SetBool("Walk", false);
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGround())
        {
            player.ChangeState(eState.JUMP);
            return;
        }

        if (isAllKeyUp)
        {
            if (delayMillisec <= stopwatch.ElapsedMilliseconds)
            {
                player.ChangeState(eState.IDLE);
                return;
            }
            else
            {
                foreach (Direction direction in player.GetDirections())
                {
                    if (player.GetKeysDownDirection(direction) && player.GetDirection() == direction)
                    {
                        player.ChangeState(eState.RUN);
                        return;
                    }

                    if (player.GetKeysDownDirection(direction))
                    {
                        isAllKeyUp = false;
                        stopwatch.Stop();
                        stopwatch.Reset();
                    }
                }
            }
        }

        bool isInput = false;

        foreach (Direction direction in player.GetDirections())
        {
            if (player.GetKeysDirection(direction))
            {
                player.SetDirection(direction);

                isInput = true;
            }
        }

        if (isInput == false)
        {
            isAllKeyUp = true;
            stopwatch.Start();
        }
        else
        {
            player.MovePosition();
        }
    }
}