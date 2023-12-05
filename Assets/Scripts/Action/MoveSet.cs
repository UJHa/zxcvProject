using System;
using System.Collections.Generic;
using Animancer;
using DataClass;
using UnityEngine;
using Utils;

public enum ActionKey
{
    NONE,
    FIGHTER_IDLE,
    FIGHTER_WALK,
    FIGHTER_RUN,
    FIGHTER_RUN_STOP,
    JUMP_UP,
    JUMP_DOWN,
    LANDING,
    FIGHTER_WEEK_ATTACK1,
    FIGHTER_WEEK_ATTACK2,
    FIGHTER_WEEK_ATTACK3,
    FIGHTER_STRONG_ATTACK1,
    FIGHTER_STRONG_ATTACK2,
    FIGHTER_WEEK_AIR_ATTACK1,
    FIGHTER_WEEK_AIR_ATTACK2,
    FIGHTER_WEEK_AIR_ATTACK3,
    FIGHTER_RUN_ATTACK,
    FIGHTER_GUARD,
    FIGHTER_GUARD_DAMAGED,
    NORMAL_DAMAGED,
    AIRBORNE_DAMAGED,
    AIRBORNE_POWER_DOWN_DAMAGED,
    KNOCK_BACK_DAMAGED,
    FLY_AWAY_DAMAGED,
    DAMAGED_AIRBORNE_LOOP,
    DAMAGED_LANDING,
    WAKE_UP,
    DEAD,
    GET_ITEM,
    RAPIER_IDLE,
    RAPIER_WALK,
    RAPIER_RUN,
    RAPIER_RUN_STOP,
    RAPIER_JUMP_UP,
    RAPIER_JUMP_DOWN,
    RAPIER_LANDING,
    RAPIER_WEEK_ATTACK1,
    RAPIER_WEEK_ATTACK2,
    RAPIER_WEEK_ATTACK3,
    RAPIER_STRONG_ATTACK1,
    RAPIER_STRONG_ATTACK2,
    RAPIER_WEEK_AIR_ATTACK1,
    RAPIER_WEEK_AIR_ATTACK2,
    RAPIER_WEEK_AIR_ATTACK3,
    MAGIC_WEEK_ATTACK1,
    MAGIC_WEEK_ATTACK2,
    MAGIC_WEEK_ATTACK3,
}

public enum ActionType
{
    NONE,
    ATTACK,
    DEFENCE,
}

public enum AttackType
{
    NONE,
    NORMAL,
    AIRBORNE,
    AIR_POWER_DOWN,
    KNOCK_BACK,
    FLY_AWAY,
}

public class InputEnableInfo
{
    public KeyBindingType Type;
    public Dictionary<eRoleState, eRoleState> DetermineStateDict; // 각 가능 state가 어떤 결과로 보낼지 정하는 함수
}

public class MoveSet
{
    private Dictionary<KeyBindingType, InputEnableInfo> _inputEnableMap = new();   // [key:curState_enableKeyCode][value:Action] 필요 스펙 키 : input key, 값 (가능 state, 결정 state)
    private AnimancerComponent _animancer;

    public MoveSet()
    {
        
    }

    public void Init(GameObject player)
    {
        _animancer = player.GetComponent<AnimancerComponent>();
    }
    
    public eRoleState DetermineNextState(eRoleState state)
    {
        var bindingTypes = Enum.GetValues(typeof(KeyBindingType));
        foreach (KeyBindingType bindingType in bindingTypes)
        {
            if (false == _inputEnableMap.ContainsKey(bindingType))
                continue;
            var enableInfo = _inputEnableMap[bindingType];
            if (false == InputManager.Instance.GetButtonDown(enableInfo.Type))
                continue;
            if (false == enableInfo.DetermineStateDict.ContainsKey(state))
                continue;
            return enableInfo.DetermineStateDict[state];
        }
        return eRoleState.NONE;
    }

    public void RegisterEnableInputMap(KeyBindingType bindingType, eRoleState[] enableStates, eRoleState determineState)
    {
        if (false == _inputEnableMap.ContainsKey(bindingType))
            _inputEnableMap.Add(bindingType, new InputEnableInfo
            {
                Type = bindingType,
                DetermineStateDict = new()
            });
        var curStateDict = _inputEnableMap[bindingType].DetermineStateDict;
        foreach (var enableState in enableStates)
        {
            if (curStateDict.ContainsKey(enableState))
            {
                ReleaseLog.LogError($"Enable State Dictionary contains key! enableState({enableState}) determineState({determineState})");
                continue;
            }
            curStateDict.Add(enableState, determineState);
        }
    }

    private AnimancerState _curAnimancerState = null;
    private Action _curAction = null;

    public AnimancerState Play(Action action, float fadeTime = 0f)
    {
        _curAction = action;
        try
        {
            _curAnimancerState = _animancer.Play(action.GetClip(), fadeTime);
            _curAnimancerState.NormalizedTime = action.GetStartRatio();
            SetSpeed(action.GetSpeed());
            _curAnimancerState.Speed = action.GetSpeed();
        }
        catch (Exception e)
        {
            ReleaseLog.LogError($"Failed play animation! stateName({action.GetActionName()})clipName({action.GetClipPath()})clip({action.GetClip()})");
            return null;
        }
        return _curAnimancerState;
    }

    public bool IsAnimationFinish()
    {
        return _curAnimancerState.NormalizedTime >= _curAction.GetEndRatio();
    }

    public float GetCurNormTime()
    {
        return _curAnimancerState.NormalizedTime;
    }
    
    public void PauseAnimation()
    {
        _curAnimancerState.Speed = 0f;
    }

    public void SetAnimationEndRatio()
    {
        _curAnimancerState.NormalizedTime = _curAction.GetEndRatio();
    }

    public void SetSpeed(float argSpeed)
    {
        _curAnimancerState.Speed = argSpeed;
    }

    public bool IsCollisionEnable(AttackInfoData attackInfoData)
    {
        return _curAnimancerState.NormalizedTime >= attackInfoData.startRatio && _curAnimancerState.NormalizedTime <= attackInfoData.endRatio;
    }

    public bool IsEqualClip(Action action)
    {
        if (null == _curAnimancerState)
            return true;
        return _curAnimancerState.Clip.Equals(action.GetClip());
    }
    
    public float GetClipLength()
    {
        var lengthRatio = _curAction.GetEndRatio() - _curAction.GetStartRatio();
        return _curAnimancerState.Length * lengthRatio;
    }

    public float GetClipSpeed()
    {
        return _curAnimancerState.Speed;
    }

    public eRoleState GetRoleState(ActionKey prevState)
    {
        var actionState = ActionTable.GetData(prevState.ToString());
        if (null == actionState)
            return eRoleState.NONE;
        return UmUtil.StringToEnum<eRoleState>(actionState.roleState);
    }
}