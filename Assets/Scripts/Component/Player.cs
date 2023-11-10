using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        _directionVector = Vector3.back;
        // SettingRoleState("Magic");

        // 엄todo : MoveSet 공격 콤보 연결 노드 시스템 데이터 기반 제어 기능 개발 필요
        _moveSet.Init(gameObject);
        _moveSet.RegisterEnableInputMap(KeyBindingType.JUMP, new[] { eRoleState.IDLE, eRoleState.WALK, eRoleState.RUN }, eRoleState.JUMP_UP);
        // GROUND WEEK ATTACK
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[] { eRoleState.IDLE, eRoleState.WALK }, eRoleState.WEEK_ATTACK1);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[] { eRoleState.WEEK_ATTACK1 }, eRoleState.WEEK_ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[] { eRoleState.WEEK_ATTACK2 }, eRoleState.WEEK_ATTACK3);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[] { eRoleState.JUMP_UP }, eRoleState.AIR_WEEK_ATTACK1);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[] { eRoleState.AIR_WEEK_ATTACK1 }, eRoleState.AIR_WEEK_ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.WEEK_ATTACK, new[] { eRoleState.AIR_WEEK_ATTACK2 }, eRoleState.AIR_WEEK_ATTACK3); _moveSet.RegisterEnableInputMap(KeyBindingType.STRONG_ATTACK, new[] { eRoleState.IDLE }, eRoleState.STRONG_ATTACK1); _moveSet.RegisterEnableInputMap(KeyBindingType.STRONG_ATTACK, new[] { eRoleState.STRONG_ATTACK1 }, eRoleState.STRONG_ATTACK2);
        _moveSet.RegisterEnableInputMap(KeyBindingType.INTERACTION, new[] { eRoleState.IDLE }, eRoleState.GET_ITEM);
        _curRoleState = eRoleState.IDLE;
        _roleStateMap[_curRoleState].StartState();
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
    }
}