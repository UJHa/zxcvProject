using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        StartUI();

        _directionVector = Vector3.back;
        
        _moveSet.Init(gameObject);
        // Punch
        _moveSet.RegisterAction(eState.ATTACK, KeyCode.C, eState.IDLE, new ActionInfo(ActionType.ATTACK, "Animation/Lucy_FightFist01_1", 0f, 0.7f));
        _moveSet.RegisterAction(eState.ATTACK2, KeyCode.C, eState.ATTACK, new ActionInfo(ActionType.ATTACK, "Animation/Lucy_FightFist01_2", 0.1f, 0.5f));
        _moveSet.RegisterAction(eState.ATTACK3, KeyCode.C, eState.ATTACK2, new ActionInfo(ActionType.ATTACK, "Animation/Lucy_FightFist02_2b_1", 0f, 0.4f));
        SettingAttackInfo(eState.ATTACK, AttackRangeType.PUNCH_A, 0.15f, 0.4f, AttackType.NORMAL, 0.1f , 0.1f);
        SettingAttackInfo(eState.ATTACK2, AttackRangeType.PUNCH_A, 0.0f, 0.3f, AttackType.NORMAL, 0.1f, 0.1f);
        SettingAttackInfo(eState.ATTACK3, AttackRangeType.PUNCH_B, 0.1f, 0.2f, AttackType.AIRBORNE, 3.5f, 1f);
        // Kick
        _moveSet.RegisterAction(eState.ATTACK4, KeyCode.X, eState.IDLE, new ActionInfo(ActionType.ATTACK, "Animation/Lucy_Kick13_Root", 0f, 0.5f));
        _moveSet.RegisterAction(eState.ATTACK5, KeyCode.X, eState.ATTACK4, new ActionInfo(ActionType.ATTACK, "Animation/Lucy_Kick12_Root", 0f, 0.35f));
        SettingAttackInfo(eState.ATTACK4, AttackRangeType.KICK_B, 0.25f, 0.3f, AttackType.NORMAL, 0.2f, 0.3f);
        SettingAttackInfo(eState.ATTACK5, AttackRangeType.KICK_A, 0.15f, 0.18f, AttackType.NORMAL, 0.2f, 0.3f);
        
        _moveSet.RegisterAction(eState.NORMAL_DAMAGED, KeyCode.Z, eState.IDLE, new ActionInfo(ActionType.NONE, "Animation/Damaged", 0f, 1.0f));
        _moveSet.RegisterAction(eState.AIRBORNE_DAMAGED, KeyCode.None, eState.IDLE, new ActionInfo(ActionType.NONE, "Animation/DamageAir_Start", 0f, 0.6f));
        _moveSet.RegisterAction(eState.DAMAGED_AIRBORNE_LOOP, KeyCode.Z, eState.AIRBORNE_DAMAGED, new ActionInfo(ActionType.NONE, "Animation/DamageAir_Fall", 0f, 1f));
        _moveSet.RegisterAction(eState.DAMAGED_LANDING, KeyCode.None, eState.DAMAGED_AIRBORNE_LOOP, new ActionInfo(ActionType.NONE, "Animation/DamageAir_End_Light", 0f, 1f));
        _moveSet.RegisterAction(eState.WAKE_UP, KeyCode.None, eState.DAMAGED_LANDING, new ActionInfo(ActionType.NONE, "Animation/LyingBack_WakeUp", 0f, 1f));

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
        _stateMap.Add(eState.ATTACK4, new KickOneState(this, eState.ATTACK4));
        _stateMap.Add(eState.ATTACK5, new KickTwoState(this, eState.ATTACK5));
        _stateMap.Add(eState.NORMAL_DAMAGED, new NormalDamagedState(this, eState.NORMAL_DAMAGED));
        _stateMap.Add(eState.AIRBORNE_DAMAGED, new AirborneDamagedState(this, eState.AIRBORNE_DAMAGED));
        _stateMap.Add(eState.DAMAGED_AIRBORNE_LOOP, new DamagedAirborneLoopState(this, eState.DAMAGED_AIRBORNE_LOOP));
        _stateMap.Add(eState.DAMAGED_LANDING, new DamagedLandingState(this, eState.DAMAGED_LANDING));
        _stateMap.Add(eState.WAKE_UP, new WakeUpState(this, eState.WAKE_UP));
        _stateMap.Add(eState.DEAD, new DeadState(this, eState.DEAD));

        _curState = eState.IDLE;

        _stateMap[_curState].StartState();
    }

    private void SettingAttackInfo(eState argState, AttackRangeType attackRangeType, float argStartRate, float argEndRate, AttackType attackType, float attackHeight, float airborneUpTime)
    {
        var action = _moveSet.GetAction(argState);
        ActionType aType = action.GetActionType();
        if (aType == ActionType.ATTACK)
        {
            action.CreateAttackInfo(attackRangeType, argStartRate, argEndRate, attackType, attackHeight, airborneUpTime);
        }
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
