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

        stateMap.Add(eState.IDLE, new NpcIdleState(this));
        stateMap.Add(eState.WALK, new WalkState(this));
        stateMap.Add(eState.RUN, new NpcRunState(this));
        stateMap.Add(eState.ATTACK, new AttackState(this));
        stateMap.Add(eState.DEAD, new DeadState(this));

        _curState = eState.IDLE;

        stateMap[_curState].StartState();
    }

    protected override void StartUI()
    {
        base.StartUI();

        Vector3 sliderPos = transform.position;
        sliderPos.y += 2f;
        slider.transform.position = Camera.main.WorldToScreenPoint(sliderPos);

        sliderScale = Vector3.Magnitude(Camera.main.transform.position - GameManager.Instance.GetPlayerUIPos()) / Vector3.Magnitude(Camera.main.transform.position - sliderPos);
        slider.gameObject.transform.localScale = Vector3.one * sliderScale;
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.UpdateEnemyCount();
    }
}