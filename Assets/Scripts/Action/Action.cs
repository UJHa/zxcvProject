// Action은 하나의 Monobavior가 가진 Animator 기반으로 동작된다고 가정한다

// 객체 예상 스펙
// >> 다음 액션 객체
// >> 애니메이션 이름
// >> 애니메이션 입력 가능 시간 범위
// >> 애니메이션 타입(이후 콤보 가능 액션, 입력 가능 액션, 타격 가능 액션 등...)(개발 전까지는 후순위)
// >> 다음 애니메이션 이동 시의 fade 시간 여부?(개발 전까지는 후순위)

using System;
using System.IO;
using Animancer;
using DataClass;
using UnityEngine;

[System.Serializable]
public class ExportAction
{
    public string clipName;
    public float startRate;
    public float endRate;
}

public class Action
{
    private readonly AnimancerComponent _animancer;
    private readonly KeyCode _inputKey;
    private readonly eState _state;
    private readonly AnimationClip _animClip;
    private AnimancerState _curState;
    private ActionData _actionData;
    private AttackInfo _attackInfo;

    // Ingame 및 json 데이터 존재할 때의 Action 생성
    public Action(AnimancerComponent animancer, eState state, KeyCode inputKey)
    {
        _animancer = animancer;
        _state = state;
        _inputKey = inputKey;
        _actionData = ActionTable.GetActionData(state.ToString());
        // _actionInfo = actionInfo;
        _animClip = Resources.Load<AnimationClip>(_actionData.clipPath);
    }
    
    public Action(AnimancerComponent animancer, string clipPath)
    {
        _animancer = animancer;
        // _state = state;
        // _inputKey = inputKey;
        // _actionData = ActionTable.GetActionData(state.ToString());
        // _actionInfo = actionInfo;
        _animClip = Resources.Load<AnimationClip>(clipPath);
    }
    
    public void CreateAttackInfo(AttackRangeType attackRangeType, float damageRatio, float argStartRate, float argEndRate, AttackType attackType, float attackHeight, float airborneUpTime)
    {
        _attackInfo = new(attackRangeType, damageRatio, argStartRate, argEndRate, attackType, attackHeight, airborneUpTime);
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
        _curState.NormalizedTime = _actionData.startTimeRatio;
        return _curState;
    }
    
    public AnimancerState PlayOnly()
    {
        _curState = _animancer.Play(_animClip);
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
    
    public float GetCurPlayRate()
    {
        return _curState.NormalizedTime;
    }
    
    // action 길이 기반 현재 비율 반환(미사용)
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
        return _curState.NormalizedTime >= _attackInfo.GetStartRate() && _curState.NormalizedTime <= _attackInfo.GetEndRate();
    }

    public AttackRangeType GetHitColliderType()
    {
        return _attackInfo.GetRangeType();
    }
    
    public AttackInfo GetaAttackInfo()
    {
        return _attackInfo;
    }

    public void Export()
    {
        ExportAction exportAction = new()
        {
            clipName = _animClip.name,
            startRate = _actionData.startTimeRatio,
            endRate = _actionData.endTimeRatio
        };
        Debug.Log($"Application.dataPath({Application.dataPath})");
        // ToJson을 사용하면 JSON형태로 포멧팅된 문자열이 생성된다
        string jsonData = JsonUtility.ToJson(exportAction);
        // 데이터를 저장할 경로 지정
        string path = Path.Combine(Application.dataPath, "action.json");
        // 파일 생성 및 저장
        File.WriteAllText(path, jsonData);
    }

    public ActionType GetActionType()
    {
        return Enum.Parse<ActionType>(_actionData.actionType);
    }
}