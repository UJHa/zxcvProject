using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class NpcIdleState : State
{
    private Stopwatch stopwatch;
    private long findTimeMillisec = 1000;
    public NpcIdleState(Character character, eState eState) : base(character, eState)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        base.StartState();
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
            GameObject target = _character.FindCollisions();
            if (target != null)
            {
                _character.SetTarget(target);
                _character.ChangeState(eState.RUN);
            }

            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}