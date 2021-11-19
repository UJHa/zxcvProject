using UnityEngine;
using UnityEditor;

public class RunState : State
{
    public RunState(Player player) : base(player)
    {
    }

    public override void StartState()
    {
        animator.SetBool(player.GetRunParameter(), true);
    }

    public override void EndState()
    {
        animator.SetBool(player.GetRunParameter(), false);
    }

    public override void UpdateState()
    {
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
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
            player.SetNextState(eState.IDLE);
            return;
        }
        

        player.MovePosition();
    }
}