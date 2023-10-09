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

        _stateMap.Add(eState.FIGHTER_IDLE, new NpcIdleState(this, eState.FIGHTER_IDLE));
        _stateMap.Add(eState.FIGHTER_WALK, new WalkState(this, eState.FIGHTER_WALK));
        _stateMap.Add(eState.FIGHTER_RUN, new NpcRunState(this, eState.FIGHTER_RUN));
        _stateMap.Add(eState.FIGHTER_WEEK_ATTACK1, new AttackState(this, eState.FIGHTER_WEEK_ATTACK1));
        _stateMap.Add(eState.DEAD, new DeadState(this, eState.DEAD));

        _curState = eState.FIGHTER_IDLE;

        _stateMap[_curState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.UpdateEnemyCount();
    }
}