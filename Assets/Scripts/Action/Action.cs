// Action은 하나의 Monobavior가 가진 Animator 기반으로 동작된다고 가정한다

// 객체 예상 스펙
// >> 다음 액션 객체
// >> 애니메이션 이름
// >> 애니메이션 입력 가능 시간 범위
// >> 애니메이션 타입(이후 콤보 가능 액션, 입력 가능 액션, 타격 가능 액션 등...)(개발 전까지는 후순위)
// >> 다음 애니메이션 이동 시의 fade 시간 여부?(개발 전까지는 후순위)

using Animancer;
using UnityEngine;

public class Action
{
    private readonly MoveSet _moveSet;
    private readonly KeyCode _inputKey;
    private readonly eState _state;
    private readonly AnimationClip _animClip;
    private AnimancerState _curState;
    private readonly ActionInfo _actionInfo;
    private float _startRate;
    private float _endRate;
    private float _startCollisionRate;
    private float _endCollisionRate;

    public Action(MoveSet moveSet, eState state, KeyCode inputKey, ActionInfo actionInfo)
    {
        _moveSet = moveSet;
        _state = state;
        _inputKey = inputKey;
        _actionInfo = actionInfo;
        _animClip = Resources.Load<AnimationClip>(actionInfo.clipPath);
        _startRate = actionInfo.startAnimNormTime;
        _endRate = actionInfo.endAnimNormTime;
        _startCollisionRate = actionInfo.startCollisionNormTime;
        _endCollisionRate = actionInfo.endCollisionNormTime;
    }

    public eState GetState()
    {
        return _state;
    }

    public AnimationClip GetClip()
    {
        return _animClip;
    }

    public AnimancerState Play()
    {
        _curState = _moveSet.Play(_animClip);
        _curState.NormalizedTime = _startRate;
        return _curState;
    }

    public bool IsAnimationFinish()
    {
        return _curState.NormalizedTime >= _endRate;
    }
    
    public bool IsCollisionEnable()
    {
        return _curState.NormalizedTime >= _startCollisionRate && _curState.NormalizedTime <= _endCollisionRate;
    }

    public AttackPartColliderType GetHitColliderType()
    {
        return _actionInfo.AttackPartColliderType;
    }
    
    public AttackType GetAttackType()
    {
        return _actionInfo.attackType;
    }
}