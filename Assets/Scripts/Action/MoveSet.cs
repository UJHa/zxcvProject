using System.Collections.Generic;
using UnityEngine;

// MoveSet Action이 알아야 되는 정보
// // 
public class MoveSet
{
    private Animator _animator;
    private Dictionary<string, Action> _actionMap = new();          // [key:name][value:Action]
    private Dictionary<string, Action> _inputEnableMap = new();   // [key:curState_enableKeyCode][value:Action]
    // 이 두개는 일단 디버깅용으로 사용할거라 나중에 지우기
    private Dictionary<eState, List<Action>> _inputEnableStateMap = new();   // 현재 eState에서 변환 가능 액션 리스트 
    private Dictionary<KeyCode, List<Action>> _inputKeyMap = new();     // 입력 키 조건의 가능 액션 리스트

    public MoveSet()
    {
        
    }

    public void Init(Animator animator)
    {
        _animator = animator;
    }
    
    public void RegisterAction(string actionName, KeyCode inputKey, eState enableState, eState actionState)
    {
        if (_actionMap.ContainsKey(actionName))
        {
            Debug.LogError($"Character contains same action name[{actionName}]");
            return;
        }

        var action = new Action(actionName, inputKey, actionState, this);
        _actionMap.Add(actionName, action);

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

    public void Play(string name)
    {
        _animator.Play(name);
    }

    public string GetCurActionName()
    {
        string result = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        return result;
    }

    public eState GetCurAction(eState curState, KeyCode inputKey)
    {
        return eState.RUN;
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