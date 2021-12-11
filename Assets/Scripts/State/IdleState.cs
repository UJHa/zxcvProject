using UnityEngine;
using UnityEditor;

public class IdleState : State
{
    public IdleState(Character character) : base(character)
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
        foreach (Direction direction in character.GetDirections())
        {
            if (character.GetKeysDownDirection(direction))
            {
                character.SetDirection(direction);

                character.ChangeState(eState.WALK);
            }
        }

        if(Input.GetKeyDown(KeyCode.V) && character.IsGround())
        {
            character.ChangeState(eState.JUMP);
        }

        if (Input.GetKeyDown(KeyCode.C) && character.IsGround())
        {
            character.ChangeState(eState.ATTACK);
        }
    }
}