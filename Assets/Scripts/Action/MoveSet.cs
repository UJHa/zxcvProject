using System.Collections.Generic;
using Animancer;
using UnityEngine;

public enum ActionType
{
    NONE,
    ATTACK,
    DEFENCE,
}

public enum AttackRangeType
{
    NONE,
    PUNCH_A,
    PUNCH_B,
    KICK_A,
    KICK_B
}
public enum AttackType
{
    NONE,
    NORMAL,
    AIRBORNE,
    AIR_POWER_DOWN,
}

public class InputEnableInfo
{
    public KeyBindingType Type;
    public KeyCode KeyCode;
    public Dictionary<eState, eState> DetermineStateDict; // 각 가능 state가 어떤 결과로 보낼지 정하는 함수
}

public class MoveSet
{
    private Dictionary<eState, Action> _actionMap = new();          // [key:eState][value:Action]
    private Dictionary<KeyBindingType, InputEnableInfo> _inputEnableMap = new();   // [key:curState_enableKeyCode][value:Action] 필요 스펙 키 : input key, 값 (가능 state, 결정 state)
    private AnimancerComponent _animancer;

    public MoveSet()
    {
        
    }

    public void Init(GameObject player)
    {
        _animancer = player.GetComponent<AnimancerComponent>();
    }
    
    public void RegisterAction(eState actionState)
    {
        Action action = null;
        if (_actionMap.ContainsKey(actionState))
        {
            Debug.LogError($"[OnlyLog]Character contains same action name[{actionState}]");
            return;
        }
        action = new Action(_animancer, actionState.ToString());
        action.Init();
        _actionMap.Add(actionState, action);
    }

    public Action GetAction(eState curState)
    {
        if (false == _actionMap.ContainsKey(curState))
            return null;
        return _actionMap[curState];
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
}