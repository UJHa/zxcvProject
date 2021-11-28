using UnityEngine;
using UnityEditor;

public class RunState : State
{
    public RunState(Player player) : base(player)
    {
    }

    public override void StartState()
    {
        player.SetRunSpeed();
        animator.SetBool("Run", true);
    }

    public override void EndState()
    {
        animator.SetBool("Run", false);
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGround())
        {
            player.ChangeState(eState.JUMP);
            return;
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

        foreach (Direction direction in player.GetDirections())
        {
            if (player.GetKeysDownDirection(direction))
            {
                player.SetDirection(direction);

                isInput = true;
            }
        }

        if (isInput == false)
        {
            player.ChangeState(eState.IDLE);
        }
        else
        {
            player.MovePosition();
        }
    }
}