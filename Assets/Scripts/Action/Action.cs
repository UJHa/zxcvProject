using Animancer;
using DataClass;
using UnityEngine;
using Utils;

public class Action
{
    protected readonly AnimancerComponent _animancer;
    private readonly KeyCode _inputKey;
    private readonly eState _state;
    protected AnimationClip _animClip;
    protected AnimancerState _curState;
    private ActionData _actionData;
    private HitboxInfo _hitboxInfo;
    
    public Action(AnimancerComponent animancer, string state, KeyCode inputKey = KeyCode.None)
    {
        _animancer = animancer;
        _state = UmUtil.StringToEnum<eState>(state);
        _inputKey = inputKey;
    }

    public void Init()
    {
        _actionData = ActionTable.GetActionData(_state.ToString());
        _animClip = Resources.Load<AnimationClip>(_actionData.clipPath);
    }

    public void SetActionData(ActionData actionData)
    {
        _actionData = actionData;
    }
    
    public void CreateHitboxInfo(string hitboxKey, AttackRangeType attackRangeType, float damageRatio, float argStartRate, float argEndRate, HitboxType hitboxType, float attackHeight, float airborneUpTime)
    {
        _hitboxInfo = new(hitboxKey, attackRangeType, damageRatio, argStartRate, argEndRate, hitboxType, attackHeight, airborneUpTime);
    }

    public eState GetState()
    {
        return _state;
    }

    public AnimationClip GetClip()
    {
        return _animClip;
    }

    public AnimancerState Play(float fadeTime = 0f)
    {
        _curState = _animancer.Play(_animClip, fadeTime);
        // 엄todo : 이런 조건문은 분리된 공간에서 관리될 수 있도록 수정하기
        // play 함수에서 호출하지 않고 분리 하고 싶음
        if (null != _actionData)
        {
            _curState.NormalizedTime = _actionData.startTimeRatio;
            _curState.Speed = _actionData.speed;
        }
        return _curState;
    }

    public void SetStartRate(float argRate)
    {
        _actionData.startTimeRatio = argRate;
    }
    
    public void SetEndRate(float argRate)
    {
        _actionData.endTimeRatio = argRate;
    }
    
    public float GetStartRate()
    {
        return _actionData.startTimeRatio;
    }
    
    public float GetEndRate()
    {
        return _actionData.endTimeRatio;
    }
    
    public void GoToFirstFrame()
    {
        _curState.NormalizedTime = _actionData.startTimeRatio;
    }
    
    public void GoToEndFrame()
    {
        _curState.NormalizedTime = _actionData.endTimeRatio;
    }

    public void SetNormTime(float ratio)
    {
        _curState.NormalizedTime = ratio;
    }
    
    public float GetCurPlayRate()
    {
        return _curState.NormalizedTime;
    }
    
    public float GetLengthRate()
    {
        return (_curState.NormalizedTime - _actionData.startTimeRatio) / (_actionData.endTimeRatio - _actionData.startTimeRatio);
    }

    public bool IsAnimationFinish()
    {
        return _curState.NormalizedTime >= _actionData.endTimeRatio;
    }
    
    public bool IsCollisionEnable()
    {
        return _curState.NormalizedTime >= _hitboxInfo.GetStartRate() && _curState.NormalizedTime <= _hitboxInfo.GetEndRate();
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
}