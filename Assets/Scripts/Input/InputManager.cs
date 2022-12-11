using System.Collections.Generic;
using UnityEngine;

public enum KeyBindingType
{
    LEFT_ARROW,
    RIGHT_ARROW,
    UP_ARROW,
    DOWN_ARROW,
    JUMP,
    ATTACK
}

public class KeyInfo
{
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

    public void Init()
    {
        _inputMap.Add(KeyBindingType.UP_ARROW, new KeyInfo { keyCode = KeyCode.UpArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.DOWN_ARROW, new KeyInfo { keyCode = KeyCode.DownArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.LEFT_ARROW, new KeyInfo { keyCode = KeyCode.LeftArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.RIGHT_ARROW, new KeyInfo { keyCode = KeyCode.RightArrow, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.JUMP, new KeyInfo { keyCode = KeyCode.V, isDown = false, isHold = false, isUp = false, });
        _inputMap.Add(KeyBindingType.ATTACK, new KeyInfo { keyCode = KeyCode.C, isDown = false, isHold = false, isUp = false, });
    }

    public void Update()
    {
        foreach (var key in _inputMap.Keys)
        {
            var info = _inputMap[key];
            bool keyDown = Input.GetKeyDown(info.keyCode);
            bool keyHold = Input.GetKey(info.keyCode);
            bool keyUp = Input.GetKeyUp(info.keyCode);
            Debug.Log($"[{info.keyCode}]keyDown({keyDown})keyHold({keyHold})keyUp({keyUp})");
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
}
