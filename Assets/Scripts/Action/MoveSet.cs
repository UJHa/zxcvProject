using System.Collections.Generic;
using Animancer;
using UnityEngine;

// MoveSet Action이 알아야 되는 정보
public enum ActionType
{
    NONE,
    ATTACK,
    DEFENCE,
}

public enum AttackRangeType
{
    NONE,
    LEFT_HAND,
    RIGHT_HAND,
    LEFT_FOOT,
    RIGHT_FOOT,
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
}

public class MoveSet
{
    private Dictionary<eState, Action> _actionMap = new();          // [key:eState][value:Action]
    private Dictionary<string, Action> _inputEnableMap = new();   // [key:curState_enableKeyCode][value:Action]
    private AnimancerComponent _animancer;

    public MoveSet()
    {
        
    }

    public void Init(GameObject player)
    {
        _animancer = player.GetComponent<AnimancerComponent>();
    }
    
    public void RegisterAction(eState actionState, KeyCode inputKey, eState enableState)
    {
        if (_actionMap.ContainsKey(actionState))
        {
            Debug.LogError($"[OnlyLog]Character contains same action name[{actionState}]");
        }
        else
        {
            _actionMap.Add(actionState, new Action(_animancer, actionState, inputKey));
        }

        var action = _actionMap[actionState];

        if (KeyCode.None != inputKey)
        {
            var enableKey = $"{enableState}_{inputKey}";
            if (_inputEnableMap.ContainsKey(enableKey))
            {
                Debug.LogError($"Character's (same state+input key) Action is not only one!");
                return;
            }
            _inputEnableMap.Add(enableKey, action);
        }
    }

    public Action GetAction(eState curState)
    {
        if (false == _actionMap.ContainsKey(curState))
            return null;
        return _actionMap[curState];
    }

    public eState DetermineNextState(eState curState, KeyCode inputKey)
    {
        var enableKey = $"{curState}_{inputKey}";
        if (false == _inputEnableMap.ContainsKey(enableKey))
            return eState.NONE;
        if (false == Input.GetKeyDown(inputKey))
            return eState.NONE;
        var action = _inputEnableMap[enableKey];
        return action.GetState();
    }
    
    // MoveSet 상위(캐릭터)에서 호출
    // fadeTime을 Animation 정보에 넣어서 관리하기
    public AnimancerState Play(eState actionState)
    {
        if (false == _actionMap.ContainsKey(actionState))
        {
            Debug.LogError($"Fail Play! actionState({actionState})");
            return null;
        }
        
        return _actionMap[actionState].Play();
    }
}