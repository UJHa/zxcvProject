using System.Collections;
using UnityEngine;

public class CGPlayerMove : CGState
{
    public CGPlayerMove(CGCharacter character) : base(character)
    {

    }

    public override void StartState()
    {
        animator.SetBool("Run", true);
    }

    public override void EndState()
    {
        animator.SetBool("Run", false);
    }

    public override void UpdateState()
    {
        if (character.ArriveMousePoint())
        {
            character.ChangeState(eCGState.IDLE);
        }
        character.SetDirection();
        character.MovePosition();
    }
}