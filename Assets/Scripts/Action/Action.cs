using DataClass;
using UnityEngine;
using Utils;

public class Action
{
    private readonly ActionKey _state;
    protected AnimationClip _animClip;
    private ActionData _actionData;
    
    public Action(string state)
    {
        _state = UmUtil.StringToEnum<ActionKey>(state);
        _actionData = ActionTable.GetData(_state.ToString());
        _animClip = Resources.Load<AnimationClip>(_actionData.clipPath);
    }
    
    public void Init()
    {
        
    }

    public void SetActionData(ActionData actionData)
    {
        _actionData = actionData;
    }

    public ActionKey GetState()
    {
        return _state;
    }

    public AnimationClip GetClip()
    {
        return _animClip;
    }
    
    public float GetSpeed()
    {
        var speed = null != _actionData ? _actionData.speed : 1f;
        return speed;
    }
    
    public void SetEndRate(float argRate)
    {
        _actionData.endTimeRatio = argRate;
    }
    
    public float GetStartRatio()
    {
        var result = null != _actionData ? _actionData.startTimeRatio : 0f;
        return result;
    }
    
    public float GetEndRatio()
    {
        var result = null != _actionData ? _actionData.endTimeRatio : 1f;
        return result;
    }

    public ActionType GetActionType()
    {
        return UmUtil.StringToEnum<ActionType>(_actionData.actionType);
    }

    public string GetActionName()
    {
        return _actionData.actionName;
    }

    public string GetClipPath()
    {
        return _actionData.clipPath;
    }
}