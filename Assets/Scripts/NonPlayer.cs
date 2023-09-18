using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NonPlayer : Character
{
    void Start()
    {
        StartUI();
        slider.gameObject.SetActive(true);

        SetWalkSpeed(0.003f);
        SetRunSpeed(0.012f);

        _directionVector = Vector3.back;

        _stateMap.Add(eState.IDLE, new NpcIdleState(this, eState.IDLE));
        _stateMap.Add(eState.WALK, new WalkState(this, eState.WALK));
        _stateMap.Add(eState.RUN, new NpcRunState(this, eState.RUN));
        _stateMap.Add(eState.ATTACK, new AttackState(this, eState.ATTACK));
        _stateMap.Add(eState.DEAD, new DeadState(this, eState.DEAD));

        _curState = eState.IDLE;

        _stateMap[_curState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.UpdateEnemyCount();
    }
}