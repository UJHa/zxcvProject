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
public enum HitboxType
{
    NONE,
    NORMAL,
    AIRBORNE,
    AIR_POWER_DOWN,
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
        Action action = null;
        if (_actionMap.ContainsKey(actionState))
        {
            Debug.LogError($"[OnlyLog]Character contains same action name[{actionState}]");
            return;
        }
        action = new Action(_animancer, actionState.ToString(), inputKey);
        action.Init();
        _actionMap.Add(actionState, action);

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
}