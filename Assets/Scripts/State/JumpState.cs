using UnityEngine;
using UnityEditor;

public class JumpState : State
{
    public JumpState(Player player) : base(player)
    {

    }

    public override void StartState()
    {
        player.StartJump();
        player.GetComponent<Rigidbody>().AddForce(0.0f, 1000.0f, 0.0f);
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
        if(player.IsGround())
        {
            player.ChangeState(eState.IDLE);
        }
    }
}