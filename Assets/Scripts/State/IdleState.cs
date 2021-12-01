using UnityEngine;
using UnityEditor;

public class IdleState : State
{
    public IdleState(Player player) : base(player)
    {

    }

    public override void StartState()
    {

    }

    public override void EndState()
    {

    }

    public override void UpdateState()
    {
        foreach (Direction direction in player.GetDirections())
        {
            if (player.GetKeysDownDirection(direction))
            {
                player.SetDirection(direction);

                player.ChangeState(eState.WALK);
            }
        }

        if(Input.GetKeyDown(KeyCode.V) && player.IsGround())
        {
            player.ChangeState(eState.JUMP);
        }
    }
}