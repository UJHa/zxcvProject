using UnityEngine;
using UnityEditor;

public class DeadState : State
{
    public DeadState(Character character, eState eState) : base(character, eState)
    {

    }

    public override void StartState()
    {
        base.StartState();
        _action.Play();
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {

    }

    public override void UpdateState()
    {
        
    }
}