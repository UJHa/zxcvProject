using UnityEngine;
using UnityEditor;

public class RunState : State
{
    public RunState(Player player) : base(player)
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
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGround())
        {
            player.ChangeState(eState.JUMP);
            return;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
            player.SetDirection(Direction.LEFT_FRONT);
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
            player.SetDirection(Direction.RIGHT_FRONT);
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            player.SetDirection(Direction.LEFT_BACK);
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            player.SetDirection(Direction.RIGHT_BACK);
        else if (Input.GetKey(KeyCode.DownArrow))
            player.SetDirection(Direction.FRONT);
        else if (Input.GetKey(KeyCode.UpArrow))
            player.SetDirection(Direction.BACK);
        else if (Input.GetKey(KeyCode.LeftArrow))
            player.SetDirection(Direction.LEFT);
        else if (Input.GetKey(KeyCode.RightArrow))
            player.SetDirection(Direction.RIGHT);
        else
        {
            //방향 키 입력 없을 때 기본(IDLE) 상태로 변경
            player.ChangeState(eState.IDLE);
            return;
        }
        

        player.MovePosition();
    }
}