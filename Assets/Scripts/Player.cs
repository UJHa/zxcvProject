using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        _directionVector = Vector3.back;
        
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

    public override void DeadDisable()
    {
        base.DeadDisable();
    }
}
