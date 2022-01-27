using System.Collections;
using UnityEngine;

public class CGPlayerIdle : CGState
{
    public CGPlayerIdle(CGCharacter character) : base(character)
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
        if (!character.ArriveMousePoint())
        {
            character.ChangeState(eCGState.MOVE);
        }
    }
}