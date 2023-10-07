using System;
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
        // 이거를 게임 도중에 할 수도 있음.. 겟앰프드의 야수 캐릭터 같은 경우? or 캐릭터 체력 상태별 다른 공격 모션을 주고 싶을 때
        // 엄todo : Character 생성 시 등록되어야 할 Action List 가져오는 제어 도구 개발 필요
        RegisterState(eState.IDLE, typeof(IdleState));
        RegisterState(eState.WALK, typeof(WalkState));
        RegisterState(eState.RUN, typeof(RunState));
        RegisterState(eState.RUN_STOP, typeof(RunStopState));
        RegisterState(eState.JUMP_UP, typeof(JumpUpState));
        RegisterState(eState.JUMP_DOWN, typeof(JumpDownState));
        RegisterState(eState.LANDING, typeof(LandingState));

        RegisterState(eState.ATTACK, typeof(PunchOneState));
        RegisterState(eState.ATTACK2, typeof(PunchTwoState));
        RegisterState(eState.ATTACK3, typeof(PunchThreeState));
        RegisterState(eState.ATTACK4, typeof(KickOneState));
        RegisterState(eState.ATTACK5, typeof(KickTwoState));
        RegisterState(eState.FIGHTER_AIR_ATTACK1, typeof(AirAttackOneState));
        RegisterState(eState.FIGHTER_AIR_ATTACK2, typeof(AirAttackTwoState));
        RegisterState(eState.FIGHTER_AIR_ATTACK3, typeof(AirAttackThreeState));

        RegisterState(eState.NORMAL_DAMAGED, typeof(NormalDamagedState));
        RegisterState(eState.AIRBORNE_DAMAGED, typeof(AirborneDamagedState));
        RegisterState(eState.AIRBORNE_POWER_DOWN_DAMAGED, typeof(AirBornePowerDownDamagedState));
        RegisterState(eState.DAMAGED_AIRBORNE_LOOP, typeof(DamagedAirborneLoopState));
        RegisterState(eState.DAMAGED_LANDING, typeof(DamagedLandingState));
        
        RegisterState(eState.WAKE_UP, typeof(WakeUpState));
        RegisterState(eState.DEAD, typeof(DeadState));
        
        RegisterState(eState.GET_ITEM, typeof(GetItemState));
        
        // 엄todo : MoveSet 공격 콤보 연결 노드 시스템 데이터 기반 제어 기능 개발 필요
        _moveSet.RegisterEnableInputMap(KeyBindingType.JUMP, new[]{eState.IDLE, eState.WALK, eState.RUN}, eState.JUMP_UP);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.IDLE}, eState.ATTACK);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.ATTACK}, eState.ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.ATTACK2}, eState.ATTACK3);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.JUMP_UP, eState.FIGHTER_AIR_ATTACK3}, eState.FIGHTER_AIR_ATTACK1);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_AIR_ATTACK1}, eState.FIGHTER_AIR_ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_AIR_ATTACK2}, eState.FIGHTER_AIR_ATTACK3);
        _moveSet.RegisterEnableInputMap(KeyBindingType.STRONG_ATTACK, new[]{eState.IDLE}, eState.ATTACK4);
        _moveSet.RegisterEnableInputMap(KeyBindingType.STRONG_ATTACK, new[]{eState.ATTACK4}, eState.ATTACK5);
        _moveSet.RegisterEnableInputMap(KeyBindingType.INTERACTION, new[]{eState.IDLE}, eState.GET_ITEM);

        SettingAttackInfo(eState.ATTACK, AttackRangeType.PUNCH_A, 1f, 0.15f, 0.4f, AttackType.NORMAL, 0.1f , 0.2f);
        SettingAttackInfo(eState.ATTACK2, AttackRangeType.PUNCH_A, 1f, 0.0f, 0.3f, AttackType.NORMAL, 0.1f, 0.2f);
        SettingAttackInfo(eState.ATTACK3, AttackRangeType.PUNCH_B, 1f, 0.1f, 0.2f, AttackType.AIRBORNE, 3.5f, 1f);
        SettingAttackInfo(eState.ATTACK4, AttackRangeType.KICK_B, 1f, 0.25f, 0.3f, AttackType.NORMAL, 0.2f, 0.3f);
        SettingAttackInfo(eState.ATTACK5, AttackRangeType.KICK_A, 1f, 0.15f, 0.18f, AttackType.NORMAL, 0.2f, 0.3f);
        SettingAttackInfo(eState.FIGHTER_AIR_ATTACK1, AttackRangeType.PUNCH_A, 1f, 0f, 1f, AttackType.NORMAL, 0.1f, 0.2f);
        SettingAttackInfo(eState.FIGHTER_AIR_ATTACK2, AttackRangeType.PUNCH_A, 1f, 0f, 1f, AttackType.NORMAL, 0.1f, 0.2f);
        SettingAttackInfo(eState.FIGHTER_AIR_ATTACK3, AttackRangeType.PUNCH_A, 1f, 0f, 1f, AttackType.AIR_POWER_DOWN, 0.0f, 0.0f);

        _curState = eState.IDLE;

        _stateMap[_curState].StartState();
    }

    private void RegisterState(eState state, Type type)
    {
        _moveSet.RegisterAction(state);
        State a = Activator.CreateInstance(type, this, state) as State;
        _stateMap.Add(state, a);
    }

    protected override void InitStats()
    {
        _hp = 5f;
        _mp = 5f;
        _strength = 5f;
        _agility = 5f;
        _intellect = 5f;
        CalculateStats();
    }

    private void SettingAttackInfo(eState argState, AttackRangeType attackRangeType, float damageRatio, float argStartRate, float argEndRate, AttackType attackType, float attackHeight, float airborneUpTime)
    {
        var action = _moveSet.GetAction(argState);
        ActionType aType = action.GetActionType();
        if (aType == ActionType.ATTACK)
        {
            
            action.CreateHitboxInfo($"{GetInstanceID()}_{argState}", attackRangeType, damageRatio, argStartRate, argEndRate, attackType, attackHeight, airborneUpTime);
        }
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.OpenFinishDialog("실패");
    }
}
