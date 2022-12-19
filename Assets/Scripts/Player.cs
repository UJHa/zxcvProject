using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        StartUI();

        // SetWalkSpeed(0.005f);
        // SetRunSpeed(0.015f);
        // Vector3 temp = new Vector3(0, 0, 1);    //Direction.FRONT
        // Vector3 temp = new Vector3(0, 0, -1);    //Direction.BACK
        // Vector3 temp = new Vector3(1, 0, 0);    //Direction.LEFT
        // Vector3 temp = new Vector3(-1, 0, 0);    //Direction.RIGHT
        // Vector3 temp = new Vector3(1, 0, 1);    //Direction.LEFT_FRONT
        // Vector3 temp = new Vector3(1, 0, -1);    //Direction.LEFT_BACK
        // Vector3 temp = new Vector3(-1, 0, 1);    //Direction.RIGHT_FRONT
        // Vector3 temp = new Vector3(-1, 0, -1);    //Direction.RIGHT_BACK
        
        // Vector3 temp = new Vector3(0, 0, -1);    //Direction.RIGHT
        // var temp2 = Quaternion.LookRotation(temp);
        // var temp3 = temp2.eulerAngles;
        // Debug.Log($"[onlyTestUm]vec({temp}), lookRot({temp2}), euler({temp3})");

        _directionVector = Vector3.back;

        stateMap.Add(eState.IDLE, new IdleState(this));
        stateMap.Add(eState.WALK, new WalkState(this));
        stateMap.Add(eState.RUN, new RunState(this));
        stateMap.Add(eState.JUMP_UP, new JumpUpState(this));
        stateMap.Add(eState.JUMP_DOWN, new JumpDownState(this));
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
        GameManager.Instance.SetPlayerUIPos(sliderPos);
        slider.transform.position = Camera.main.WorldToScreenPoint(sliderPos);
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        Vector3 sliderPos = transform.position;
        sliderPos.y += 2f;
        GameManager.Instance.SetPlayerUIPos(transform.position);
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.OpenFinishDialog("실패");
    }
}
