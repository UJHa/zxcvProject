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
        if (Input.GetKey(KeyCode.UpArrow) || (Input.GetKey(KeyCode.DownArrow)) || (Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.RightArrow)))
        {
            player.ChangeState(eState.RUN);
        }
        else if(Input.GetKeyDown(KeyCode.Space) && player.IsGround())
        {
            player.ChangeState(eState.JUMP);
        }
    }
}