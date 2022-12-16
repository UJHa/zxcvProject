using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum KeyBindingType
{
    NONE,
    LEFT_ARROW,
    RIGHT_ARROW,
    UP_ARROW,
    DOWN_ARROW,
    JUMP,
    ATTACK
}

public class KeyInfo
{
    public KeyBindingType keyType;
    public KeyCode keyCode;
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
            Debug.LogError($"InputManager 존재합니다!");
        }
        Instance = new InputManager();
        IsExistInstance = true;
    }
    
    private Dictionary<KeyBindingType, KeyInfo> _inputMap = new ();
    private Queue<KeyBindingType> _downQueue = new();
    private Queue<KeyBindingType> _upQueue = new();
    private int _downMaxSize = 5;
    private int _upMaxSize = 5;

    public void Init()
    {
        _inputMap.Add(KeyBindingType.UP_ARROW, new KeyInfo { keyType = KeyBindingType.UP_ARROW, keyCode = KeyCode.UpArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.DOWN_ARROW, new KeyInfo { keyType = KeyBindingType.DOWN_ARROW, keyCode = KeyCode.DownArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.LEFT_ARROW, new KeyInfo { keyType = KeyBindingType.LEFT_ARROW, keyCode = KeyCode.LeftArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.RIGHT_ARROW, new KeyInfo { keyType = KeyBindingType.RIGHT_ARROW, keyCode = KeyCode.RightArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.JUMP, new KeyInfo { keyType = KeyBindingType.JUMP, keyCode = KeyCode.V, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.ATTACK, new KeyInfo { keyType = KeyBindingType.ATTACK, keyCode = KeyCode.C, isDown = false, isHold = false, isUp = false, });
        
    }

    public void Update()
    {
        foreach (var key in _inputMap.Keys)
        {
            var info = _inputMap[key];
            _inputMap[key].isDown = Input.GetKeyDown(info.keyCode); 
            _inputMap[key].isHold = Input.GetKey(info.keyCode); 
            _inputMap[key].isUp = Input.GetKeyUp(info.keyCode); 
            // Debug.Log($"[{info.keyCode}]keyDown({_inputMap[key].isDown})keyHold({_inputMap[key].isHold})keyUp({_inputMap[key].isUp})");
            SaveQueue(_inputMap[key]);
        }
        
        // var horAxisRaw = Input.GetAxisRaw("Horizontal"); // true false
        // var verAxisRaw = Input.GetAxisRaw("Vertical");   // true false
        // Debug.Log($"hor({horAxisRaw})ver({verAxisRaw})");
    }

    private void SaveQueue(KeyInfo input)
    {
        if (input.isDown)
        {
            if (_downQueue.Count <= _downMaxSize)
            {
                _downQueue.Enqueue(input.keyType);
            }
            else
            {
                _downQueue.Dequeue();
            }
        }
        
        if (input.isUp)
        {
            if (_upQueue.Count <= _upMaxSize)
            {
                _upQueue.Enqueue(input.keyType);
            }
            else
            {
                _upQueue.Dequeue();
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
        return origin.normalized;
    }

    public KeyBindingType GetLastKeyUp()
    {
        if (_upQueue.Count == 0)
            return KeyBindingType.NONE;
        return _upQueue.Last();
    }
    
    public KeyBindingType GetLastKeyDown()
    {
        if (_downQueue.Count == 0)
            return KeyBindingType.NONE;
        return _downQueue.Last();
    }
}
