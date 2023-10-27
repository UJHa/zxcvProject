using System;
using System.Collections.Generic;
using Animancer;
using DataClass;
using UnityEngine;
using Utils;

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
    public KeyCode KeyCode;
    public Dictionary<eState, eState> DetermineStateDict; // 각 가능 state가 어떤 결과로 보낼지 정하는 함수
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
    
    public eState DetermineNextState(eState state, KeyBindingType inputBindingType)
    {
        if (false == _inputEnableMap.ContainsKey(inputBindingType))
            return eState.NONE;
        var enableInfo = _inputEnableMap[inputBindingType];
        if (false == Input.GetKeyDown(enableInfo.KeyCode))
            return eState.NONE;
        if (false == enableInfo.DetermineStateDict.ContainsKey(state))
            return eState.NONE;
        return enableInfo.DetermineStateDict[state];
    }

    public void RegisterEnableInputMap(KeyBindingType bindingType, eState[] enableStates, eState determineState)
    {
        if (false == _inputEnableMap.ContainsKey(bindingType))
            _inputEnableMap.Add(bindingType, new InputEnableInfo
            {
                Type = bindingType,
                KeyCode = InputManager.Instance.GetKeyCode(bindingType),
                DetermineStateDict = new()
            });
        var curStateDict = _inputEnableMap[bindingType].DetermineStateDict;
        foreach (var enableState in enableStates)
        {
            if (curStateDict.ContainsKey(enableState))
            {
                Debug.LogError($"Enable State Dictionary contains key! enableState({enableState}) determineState({determineState})");
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
            Debug.LogError($"Failed play animation! stateName({action.GetActionName()})clipName({action.GetClipPath()})clip({action.GetClip()})");
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

    public eRoleState GetRoleState(eState prevState)
    {
        var actionState = ActionTable.GetData(prevState.ToString());
        if (null == actionState)
            return eRoleState.NONE;
        return UmUtil.StringToEnum<eRoleState>(actionState.roleState);
    }
}