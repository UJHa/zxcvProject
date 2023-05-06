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

        // 이거를 게임 도중에 할 수도 있음.. 겟앰프드의 야수 캐릭터 같은 경우? or 캐릭터 체력 상태별 다른 공격 모션을 주고 싶을 때
        _stateMap.Add(eState.IDLE, new IdleState(this, eState.IDLE));
        _stateMap.Add(eState.WALK, new WalkState(this, eState.WALK));
        _stateMap.Add(eState.RUN, new RunState(this, eState.RUN));
        _stateMap.Add(eState.RUNSTOP, new RunStopState(this, eState.RUNSTOP));
        _stateMap.Add(eState.JUMP_UP, new JumpUpState(this, eState.JUMP_UP));
        _stateMap.Add(eState.JUMP_DOWN, new JumpDownState(this, eState.JUMP_DOWN));
        _stateMap.Add(eState.LANDING, new LandingState(this, eState.LANDING));
        _stateMap.Add(eState.ATTACK, new PunchOneState(this, eState.ATTACK));
        _stateMap.Add(eState.ATTACK2, new PunchTwoState(this, eState.ATTACK2));
        _stateMap.Add(eState.ATTACK3, new PunchThreeState(this, eState.ATTACK3));
        _stateMap.Add(eState.DEAD, new DeadState(this, eState.DEAD));

        var animator = GetComponent<Animator>();
        if (null == animator)
        {
            Debug.LogError($"Player don't have animator!");
            return;
        }
        _moveSet.Init(animator);
        _moveSet.RegisterAction("PunchCombo", KeyCode.C, eState.IDLE, eState.ATTACK);
        // _moveSet.RegisterAction("Punch2");
        // _moveSet.RegisterAction("Punch3");

        _curState = eState.IDLE;

        _stateMap[_curState].StartState();
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
