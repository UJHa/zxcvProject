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
        // Add to MoveSet
        _moveSet.RegisterAction(eState.IDLE, KeyCode.None, eState.NONE);
        _moveSet.RegisterAction(eState.WALK, KeyCode.None, eState.NONE);
        _moveSet.RegisterAction(eState.RUN, KeyCode.None, eState.NONE);
        _moveSet.RegisterAction(eState.RUN_STOP, KeyCode.None, eState.NONE);
        _moveSet.RegisterAction(eState.JUMP_UP, KeyCode.None, eState.NONE);
        _moveSet.RegisterAction(eState.JUMP_DOWN, KeyCode.None, eState.NONE);
        _moveSet.RegisterAction(eState.LANDING, KeyCode.None, eState.NONE);
        // Punch
        _moveSet.RegisterAction(eState.ATTACK, KeyCode.C, eState.IDLE);
        _moveSet.RegisterAction(eState.ATTACK2, KeyCode.C, eState.ATTACK);
        _moveSet.RegisterAction(eState.ATTACK3, KeyCode.C, eState.ATTACK2);
        // Kick
        _moveSet.RegisterAction(eState.ATTACK4, KeyCode.X, eState.IDLE);
        _moveSet.RegisterAction(eState.ATTACK5, KeyCode.X, eState.ATTACK4);
        // Air Punch
        _moveSet.RegisterAction(eState.FIGHTER_AIR_ATTACK1, KeyCode.C, eState.JUMP_UP);
        _moveSet.RegisterAction(eState.FIGHTER_AIR_ATTACK2, KeyCode.C, eState.FIGHTER_AIR_ATTACK1);
        _moveSet.RegisterAction(eState.FIGHTER_AIR_ATTACK3, KeyCode.C, eState.FIGHTER_AIR_ATTACK2);
        // Damaged
        _moveSet.RegisterAction(eState.NORMAL_DAMAGED, KeyCode.Z, eState.IDLE);
        _moveSet.RegisterAction(eState.AIRBORNE_DAMAGED, KeyCode.None, eState.IDLE);
        _moveSet.RegisterAction(eState.DAMAGED_AIRBORNE_LOOP, KeyCode.Z, eState.AIRBORNE_DAMAGED);
        _moveSet.RegisterAction(eState.DAMAGED_LANDING, KeyCode.None, eState.DAMAGED_AIRBORNE_LOOP);
        
        _moveSet.RegisterAction(eState.WAKE_UP, KeyCode.None, eState.DAMAGED_LANDING);
        _moveSet.RegisterAction(eState.DEAD, KeyCode.None, eState.NONE);
        
        SettingAttackInfo(eState.ATTACK, AttackRangeType.PUNCH_A, 1f, 0.15f, 0.4f, AttackType.NORMAL, 0.1f , 0.2f);
        SettingAttackInfo(eState.ATTACK2, AttackRangeType.PUNCH_A, 1f, 0.0f, 0.3f, AttackType.NORMAL, 0.1f, 0.2f);
        SettingAttackInfo(eState.ATTACK3, AttackRangeType.PUNCH_B, 1f, 0.1f, 0.2f, AttackType.AIRBORNE, 3.5f, 1f);
        SettingAttackInfo(eState.ATTACK4, AttackRangeType.KICK_B, 1f, 0.25f, 0.3f, AttackType.NORMAL, 0.2f, 0.3f);
        SettingAttackInfo(eState.ATTACK5, AttackRangeType.KICK_A, 1f, 0.15f, 0.18f, AttackType.NORMAL, 0.2f, 0.3f);
        SettingAttackInfo(eState.FIGHTER_AIR_ATTACK1, AttackRangeType.PUNCH_A, 1f, 0f, 1f, AttackType.NORMAL, 0.1f, 0.3f);
        SettingAttackInfo(eState.FIGHTER_AIR_ATTACK2, AttackRangeType.PUNCH_A, 1f, 0f, 1f, AttackType.NORMAL, 0.1f, 0.3f);
        SettingAttackInfo(eState.FIGHTER_AIR_ATTACK3, AttackRangeType.PUNCH_A, 1f, 0f, 1f, AttackType.NORMAL, 0.0f, 0.0f);
        

        // 이거를 게임 도중에 할 수도 있음.. 겟앰프드의 야수 캐릭터 같은 경우? or 캐릭터 체력 상태별 다른 공격 모션을 주고 싶을 때
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
        
        RegisterState(eState.NORMAL_DAMAGED, typeof(NormalDamagedState));
        RegisterState(eState.AIRBORNE_DAMAGED, typeof(AirborneDamagedState));
        RegisterState(eState.DAMAGED_AIRBORNE_LOOP, typeof(DamagedAirborneLoopState));
        RegisterState(eState.DAMAGED_LANDING, typeof(DamagedLandingState));
        RegisterState(eState.WAKE_UP, typeof(WakeUpState));
        RegisterState(eState.DEAD, typeof(DeadState));
        
        RegisterState(eState.FIGHTER_AIR_ATTACK1, typeof(AirAttackOne));
        RegisterState(eState.FIGHTER_AIR_ATTACK2, typeof(AirAttackTwo));
        RegisterState(eState.FIGHTER_AIR_ATTACK3, typeof(AirAttackThree));

        _curState = eState.IDLE;

        _stateMap[_curState].StartState();
    }

    private void RegisterState(eState state, Type type)
    {
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
            action.CreateAttackInfo(attackRangeType, damageRatio, argStartRate, argEndRate, attackType, attackHeight, airborneUpTime);
        }
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.OpenFinishDialog("실패");
    }
}
