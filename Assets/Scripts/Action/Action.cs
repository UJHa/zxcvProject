// Action은 하나의 Monobavior가 가진 Animator 기반으로 동작된다고 가정한다

// 객체 예상 스펙
// >> 다음 액션 객체
// >> 애니메이션 이름
// >> 애니메이션 입력 가능 시간 범위
// >> 애니메이션 타입(이후 콤보 가능 액션, 입력 가능 액션, 타격 가능 액션 등...)(개발 전까지는 후순위)
// >> 다음 애니메이션 이동 시의 fade 시간 여부?(개발 전까지는 후순위)

using System.IO;
using Animancer;
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
    private ActionInfo _actionInfo;
    private AttackInfo _attackInfo;

    public Action(AnimancerComponent animancer, eState state, KeyCode inputKey, ActionInfo actionInfo)
    {
        _animancer = animancer;
        _state = state;
        _inputKey = inputKey;
        _actionInfo = actionInfo;
        _animClip = Resources.Load<AnimationClip>(actionInfo.GetClipPath());
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
        _curState.NormalizedTime = _actionInfo.GetStartRate();
        return _curState;
    }
    
    public AnimancerState PlayOnly()
    {
        _curState = _animancer.Play(_animClip);
        return _curState;
    }

    public void SetStartRate(float argRate)
    {
        _actionInfo.SetStartRate(argRate);
    }
    
    public void SetEndRate(float argRate)
    {
        _actionInfo.SetEndRate(argRate);
    }
    
    public float GetStartRate()
    {
        return _actionInfo.GetStartRate();
    }
    
    public float GetEndRate()
    {
        return _actionInfo.GetEndRate();
    }
    
    public void GoToFirstFrame()
    {
        _curState.NormalizedTime = _actionInfo.GetStartRate();
    }
    
    public void GoToEndFrame()
    {
        _curState.NormalizedTime = _actionInfo.GetEndRate();
    }
    
    public float GetCurPlayRate()
    {
        return _curState.NormalizedTime;
    }
    
    // action 길이 기반 현재 비율 반환(미사용)
    public float GetLengthRate()
    {
        return (_curState.NormalizedTime - _actionInfo.GetStartRate()) / _actionInfo.GetRateLength();
    }

    public bool IsAnimationFinish()
    {
        return _curState.NormalizedTime >= _actionInfo.GetEndRate();
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
            startRate = _actionInfo.GetStartRate(),
            endRate = _actionInfo.GetEndRate()
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
        return _actionInfo.GetActionType();
    }
}