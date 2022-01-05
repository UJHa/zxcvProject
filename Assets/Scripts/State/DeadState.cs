using UnityEngine;
using UnityEditor;

public class DeadState : State
{
    public DeadState(Character character) : base(character)
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
        character.DeadDisable();
    }
}