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
    // 이 두개는 일단 디버깅용으로 사용할거라 나중에 지우기
    private Dictionary<eState, List<Action>> _inputEnableStateMap = new();   // 현재 eState에서 변환 가능 액션 리스트 
    private Dictionary<KeyCode, List<Action>> _inputKeyMap = new();     // 입력 키 조건의 가능 액션 리스트
    private AnimancerComponent _animancer;

    public MoveSet()
    {
        
    }

    public void Init(GameObject player)
    {
        _animancer = player.GetComponent<AnimancerComponent>();
    }
    
    public void RegisterAction(eState actionState, KeyCode inputKey, eState enableState, ActionInfo actionInfo)
    {
        if (_actionMap.ContainsKey(actionState))
        {
            Debug.LogError($"[OnlyLog]Character contains same action name[{actionState}]");
        }
        else
        {
            _actionMap.Add(actionState, new Action(_animancer, actionState, inputKey, actionInfo));
        }

        var action = _actionMap[actionState];

        var enableKey = $"{enableState}_{inputKey}";
        if (_inputEnableMap.ContainsKey(enableKey))
        {
            Debug.LogError($"Character's (same state+input key) Action is not only one!");
            return;
        }
        _inputEnableMap.Add(enableKey, action);
        
        // 이 밑의 두개는 일단 디버깅용 불필요하면 지우자.
        if (false == _inputEnableStateMap.ContainsKey(enableState))
            _inputEnableStateMap.Add(enableState, new());
        _inputEnableStateMap[enableState].Add(action);
        
        if (false == _inputKeyMap.ContainsKey(inputKey))
            _inputKeyMap.Add(inputKey, new());
        _inputKeyMap[inputKey].Add(action);
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