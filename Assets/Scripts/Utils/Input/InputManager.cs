using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

// 키 입력 우선순위 기준으로 처리해야됨(다중 키가 enum 앞번호여야 함)
public enum KeyBindingType
{
    NONE,
    GUARD,
    LEFT_ARROW,
    RIGHT_ARROW,
    UP_ARROW,
    DOWN_ARROW,
    JUMP,
    WEEK_ATTACK,
    STRONG_ATTACK,
    INTERACTION
}

public class KeyInfo
{
    public KeyBindingType keyType;
    public KeyCode[] keyCodes;
    public bool isDown = false;
    public bool isHold = false;
    public bool isUp = false;
}

public class InputManager
{
    public static InputManager Instance = null;
    public static bool IsExistInstance = false;

    public static void CreateInstance()
    {
        if (null != Instance)
        {
            ReleaseLog.LogError($"InputManager 존재합니다!");
        }
        Instance = new InputManager();
        IsExistInstance = true;
    }
    
    private Dictionary<KeyBindingType, KeyInfo> _inputMap = new ();
    private LinkedList<KeyBindingType> _downList = new();
    private LinkedList<KeyBindingType> _holdList = new();
    private LinkedList<KeyBindingType> _upList = new();
    private int _downMaxSize = 5;
    private int _holdMaxSize = 5;
    private int _upMaxSize = 5;

    public void Init()
    {
        _inputMap.Add(KeyBindingType.UP_ARROW, new KeyInfo { keyType = KeyBindingType.UP_ARROW, keyCodes = new KeyCode[] { KeyCode.UpArrow }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.DOWN_ARROW, new KeyInfo { keyType = KeyBindingType.DOWN_ARROW, keyCodes = new KeyCode[] { KeyCode.DownArrow }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.LEFT_ARROW, new KeyInfo { keyType = KeyBindingType.LEFT_ARROW, keyCodes = new KeyCode[] { KeyCode.LeftArrow }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.RIGHT_ARROW, new KeyInfo { keyType = KeyBindingType.RIGHT_ARROW, keyCodes = new KeyCode[] { KeyCode.RightArrow }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.JUMP, new KeyInfo { keyType = KeyBindingType.JUMP, keyCodes = new KeyCode[] { KeyCode.V }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.WEEK_ATTACK, new KeyInfo { keyType = KeyBindingType.WEEK_ATTACK, keyCodes = new KeyCode[] { KeyCode.C }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.STRONG_ATTACK, new KeyInfo { keyType = KeyBindingType.STRONG_ATTACK, keyCodes = new KeyCode[] { KeyCode.X }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.INTERACTION, new KeyInfo { keyType = KeyBindingType.STRONG_ATTACK, keyCodes = new KeyCode[] { KeyCode.Z }, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.GUARD, new KeyInfo { keyType = KeyBindingType.GUARD, keyCodes = new KeyCode[] { KeyCode.X, KeyCode.C }, isDown = false, isHold = false, isUp = false, });
    }

    public void Update()
    {
        foreach (var key in _inputMap.Keys)
        {
            var info = _inputMap[key];
            info.isDown = true;
            info.isHold = true;
            info.isUp = true;
        }
        foreach (var key in _inputMap.Keys)
        {
            var info = _inputMap[key];
            foreach(var keyCode in info.keyCodes)
            {
                info.isDown &= Input.GetKeyDown(keyCode);
                info.isHold &= Input.GetKey(keyCode);
                info.isUp &= Input.GetKeyUp(keyCode);
            }
            
            SaveQueue(_inputMap[key]);
        }
    }

    private void SaveQueue(KeyInfo input)
    {
        if (input.isDown)
        {
            if (false == _holdList.Contains(input.keyType))
                _holdList.AddLast(input.keyType);
            _downList.AddLast(input.keyType);
            while (_downList.Count >= _downMaxSize)
            {
                _downList.RemoveFirst();
            }
        }
        
        if (input.isUp)
        {
            foreach (var holdKeyType in _holdList)
            {
                if (input.keyType == holdKeyType)
                {
                    _holdList.Remove(input.keyType);
                    break;
                }
            }
            _upList.AddLast(input.keyType);
            while (_upList.Count >= _upMaxSize)
            {
                _upList.RemoveFirst();
            }
        }
    }

    public bool GetButtonDown(KeyBindingType keyType) 
    {
        return _inputMap[keyType].isDown;
    }
    
    public bool GetButton(KeyBindingType keyType)
    {
        return _inputMap[keyType].isHold;
    }
    
    public bool GetButtonUp(KeyBindingType keyType)
    {
        return _inputMap[keyType].isUp;
    }
    
    public Vector3 GetButtonAxisRaw()
    {
        var horAxisRaw = Input.GetAxisRaw("Horizontal"); // true false
        var verAxisRaw = Input.GetAxisRaw("Vertical");   // true false
        var origin = new Vector3(-horAxisRaw, 0, -verAxisRaw);
        var holdCount = GetArrowKeyHoldCount();
        if (Vector3.zero == origin)
        {
            if (holdCount != 0)
            {
                if (holdCount == 2)
                {
                    origin = GetDiagonalVector();
                }
                if (Vector3.zero == origin)
                {
                    var keyType = GetLastKeyHold();
                    origin = GetAxisVector(keyType);
                }
            }
        }
        // Debug.Log($"[testVector]{origin}InputCount({holdCount})");
        return origin.normalized;
    }
    
    public KeyBindingType GetLastKeyDown()
    {
        if (_downList.Count == 0)
            return KeyBindingType.NONE;
        return _downList.Last();
    }
    
    public KeyBindingType GetLastKeyHold()
    {
        if (_holdList.Count == 0)
            return KeyBindingType.NONE;
        return _holdList.Last();
    }

    public KeyBindingType GetLastKeyUp()
    {
        if (_upList.Count == 0)
            return KeyBindingType.NONE;
        return _upList.Last();
    }
    
    private int GetArrowKeyHoldCount()
    {
        int result = 0;
        foreach (var keyType in _holdList)
        {
            if (keyType == KeyBindingType.UP_ARROW
                || keyType == KeyBindingType.DOWN_ARROW
                || keyType == KeyBindingType.LEFT_ARROW
                || keyType == KeyBindingType.RIGHT_ARROW)
                result++;
        }
        return result;
    }
    
    private Vector3 GetDiagonalVector()
    {
        List<KeyBindingType> result = new();
        foreach (var keyType in _holdList)
        {
            if (keyType == KeyBindingType.UP_ARROW
                || keyType == KeyBindingType.DOWN_ARROW
                || keyType == KeyBindingType.LEFT_ARROW
                || keyType == KeyBindingType.RIGHT_ARROW)
                result.Add(keyType);
        }

        if (2 != result.Count)
            return Vector3.zero;
        var resultVector = GetAxisVector(result[0]);
        resultVector += GetAxisVector(result[1]);

        return resultVector;
    }

    private Vector3 GetAxisVector(KeyBindingType keyType)
    {
        if (keyType == KeyBindingType.UP_ARROW)
            return -Vector3.forward;
        if (keyType == KeyBindingType.DOWN_ARROW)
            return -Vector3.back;
        if (keyType == KeyBindingType.LEFT_ARROW)
            return -Vector3.left;
        if (keyType == KeyBindingType.RIGHT_ARROW)
            return -Vector3.right;
            
        return Vector3.zero;
    }
}
