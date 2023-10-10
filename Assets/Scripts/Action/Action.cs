using DataClass;
using UnityEngine;
using Utils;

public class Action
{
    private readonly eState _state;
    protected AnimationClip _animClip;
    private ActionData _actionData;
    private HitboxInfo _hitboxInfo;
    
    public Action(string state)
    {
        _state = UmUtil.StringToEnum<eState>(state);
        _actionData = ActionTable.GetActionData(_state.ToString());
        _animClip = Resources.Load<AnimationClip>(_actionData.clipPath);
    }
    
    public void Init()
    {
        
    }

    public void SetActionData(ActionData actionData)
    {
        _actionData = actionData;
    }
    
    public void CreateHitboxInfo(string hitboxKey, AttackRangeType attackRangeType, float damageRatio, float argStartRate, float argEndRate, AttackType attackType, float attackHeight, float airborneUpTime)
    {
        _hitboxInfo = new(hitboxKey, attackRangeType, damageRatio, argStartRate, argEndRate, attackType, attackHeight, airborneUpTime);
    }

    public eState GetState()
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
    
    public bool IsCollisionEnable(float normTime)
    {
        return normTime >= _hitboxInfo.GetStartRate() && normTime <= _hitboxInfo.GetEndRate();
    }

    public AttackRangeType GetHitColliderType()
    {
        return _hitboxInfo.GetRangeType();
    }
    
    public HitboxInfo GetaAttackInfo()
    {
        return _hitboxInfo;
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