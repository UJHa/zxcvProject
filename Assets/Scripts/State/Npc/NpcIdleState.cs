using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class NpcIdleState : State
{
    private Stopwatch stopwatch;
    private long findTimeMillisec = 1000;
    public NpcIdleState(Character character) : base(character)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        stopwatch.Reset();
        stopwatch.Start();
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void EndState()
    {
        stopwatch.Stop();
    }

    public override void UpdateState()
    {
        if (findTimeMillisec <= stopwatch.ElapsedMilliseconds)
        {
            GameObject target = character.FindCollisions();
            if (target != null)
            {
                character.SetTarget(target);
                character.ChangeState(eState.RUN);
            }

            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}