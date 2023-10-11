using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        StartUI();

        _directionVector = Vector3.back;
        
        // 이거를 게임 도중에 할 수도 있음.. 겟앰프드의 야수 캐릭터 같은 경우? or 캐릭터 체력 상태별 다른 공격 모션을 주고 싶을 때
        // 엄todo : Character 생성 시 등록되어야 할 Action List 가져오는 제어 도구 개발 필요
        RegisterState(eState.FIGHTER_IDLE, typeof(IdleState));
        RegisterState(eState.FIGHTER_WALK, typeof(WalkState));
        RegisterState(eState.FIGHTER_RUN, typeof(RunState));
        RegisterState(eState.FIGHTER_RUN_STOP, typeof(RunStopState));
        RegisterState(eState.JUMP_UP, typeof(JumpUpState));
        RegisterState(eState.JUMP_DOWN, typeof(JumpDownState));
        RegisterState(eState.LANDING, typeof(LandingState));

        RegisterState(eState.FIGHTER_WEEK_ATTACK1, typeof(WeekAttackState));
        RegisterState(eState.FIGHTER_WEEK_ATTACK2, typeof(WeekAttackState));
        RegisterState(eState.FIGHTER_WEEK_ATTACK3, typeof(WeekAttackState));
        RegisterState(eState.FIGHTER_STRONG_ATTACK1, typeof(StrongAttackState));
        RegisterState(eState.FIGHTER_STRONG_ATTACK2, typeof(StrongAttackState));
        RegisterState(eState.FIGHTER_WEEK_AIR_ATTACK1, typeof(WeekAirAttackState));
        RegisterState(eState.FIGHTER_WEEK_AIR_ATTACK2, typeof(WeekAirAttackState));
        RegisterState(eState.FIGHTER_WEEK_AIR_ATTACK3, typeof(WeekAirAttackState));

        RegisterState(eState.NORMAL_DAMAGED, typeof(NormalDamagedState));
        RegisterState(eState.AIRBORNE_DAMAGED, typeof(AirborneDamagedState));
        RegisterState(eState.AIRBORNE_POWER_DOWN_DAMAGED, typeof(AirBornePowerDownDamagedState));
        RegisterState(eState.KNOCK_BACK_DAMAGED, typeof(KnockBackDamagedState));
        RegisterState(eState.DAMAGED_AIRBORNE_LOOP, typeof(DamagedAirborneLoopState));
        RegisterState(eState.DAMAGED_LANDING, typeof(DamagedLandingState));
        
        RegisterState(eState.WAKE_UP, typeof(WakeUpState));
        RegisterState(eState.DEAD, typeof(DeadState));
        
        RegisterState(eState.GET_ITEM, typeof(GetItemState));
        
        // 엄todo : MoveSet 공격 콤보 연결 노드 시스템 데이터 기반 제어 기능 개발 필요
        _moveSet.Init(gameObject);
        _moveSet.RegisterEnableInputMap(KeyBindingType.JUMP, new[] { eState.FIGHTER_IDLE, eState.FIGHTER_WALK, eState.FIGHTER_RUN }, eState.JUMP_UP);
        // GROUND WEEK ATTACK
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_IDLE}, eState.FIGHTER_WEEK_ATTACK1);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_WEEK_ATTACK1}, eState.FIGHTER_WEEK_ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_WEEK_ATTACK2}, eState.FIGHTER_WEEK_ATTACK3);
        // AIR WEEK ATTACK
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.JUMP_UP}, eState.FIGHTER_WEEK_AIR_ATTACK1);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_WEEK_AIR_ATTACK1}, eState.FIGHTER_WEEK_AIR_ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[]{eState.FIGHTER_WEEK_AIR_ATTACK2}, eState.FIGHTER_WEEK_AIR_ATTACK3);
        // GROUND STRONG ATTACK
        _moveSet.RegisterEnableInputMap(KeyBindingType.STRONG_ATTACK, new[]{eState.FIGHTER_IDLE}, eState.FIGHTER_STRONG_ATTACK1);
        _moveSet.RegisterEnableInputMap(KeyBindingType.STRONG_ATTACK, new[]{eState.FIGHTER_STRONG_ATTACK1}, eState.FIGHTER_STRONG_ATTACK2);
        
        _moveSet.RegisterEnableInputMap(KeyBindingType.INTERACTION, new[]{eState.FIGHTER_IDLE}, eState.GET_ITEM);
        _curState = eState.FIGHTER_IDLE;

        _stateMap[_curState].StartState();
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

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.OpenFinishDialog("실패");
    }
}
