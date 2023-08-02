using System;
using Animancer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

// 엄todo: 기능 개발 요구사항
// 캐릭터 액션 기반 처리하기
// >> 추가 작업 : Slider min,max를 action 객체가 가진 start,end 비율로 변경
// 충돌체 붙일 수 있도록 세팅 가져오기
// 캐릭터의 로직에 있는 데이터를 상위 Manager로 관리하기
public class MoveSetCharacter : MonoBehaviour
{
    private AnimancerState _curState = null;

    private AnimancerComponent _animancer;
    private Action _action = null;
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
    }
    
    public void Init(float animStartTime, float animEndTime)
    {
        ChangeAction("Animation/Lucy_FightFist01_2", animStartTime, animEndTime);
    }

    public void ChangeAction(string clipName, float animStartTime, float animEndTime)
    {
        ActionInfo actionInfo = new(clipName, animStartTime, animEndTime, AttackRangeType.NONE,
            0.0f, 0.3f, null);
        _action = new Action(_animancer, eState.NONE, KeyCode.None, actionInfo);
        _curState = _action.PlayOnly();
        // _action.GoToFirstFrame();
        PauseAnim();
    }

    private float _startMousePosX = 0f;
    private Vector3 _startRot = new();

    private void Update()
    {
        if (UIManager.Instance.IsRaycastUI())
            return;
        if (UmUtil.IsSliderHold())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log($"[testum]mousePos({Input.mousePosition})");
            _startMousePosX = Input.mousePosition.x;
            _startRot = transform.eulerAngles;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log($"[testum]mousePos({Input.mousePosition})");
        }
        else if (Input.GetMouseButton(0))
        {
            var movePosX = _startMousePosX - Input.mousePosition.x;
            // Debug.Log($"[testum]mousePos({Input.mousePosition}) movePosX({movePosX})");
            transform.eulerAngles = _startRot + new Vector3(0f, movePosX, 0f);
        }
            
        if (Input.GetKeyDown(KeyCode.X))
        {
            _action.GoToFirstFrame();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _curState.IsPlaying = !_curState.IsPlaying;
        }
        // // 엄todo : UI 레이어 Raycast 통한 무시처리로 변경하기
        // if (false == UmUtil.IsSliderHold())
        // {
        //     
        // }
        // else
        // {
        //     PauseAnim();
        // }
    }

    public void PlayAnim()
    {
        if (IsPlayFinish())
            _action.GoToFirstFrame();
        _curState = _action.PlayOnly();
    }

    public void PlayPinAnim()
    {
        if (IsPlayFinish())
            _action.GoToFirstFrame();
        _curState = _action.PlayOnly();
    }

    public void PauseAnim()
    {
        _curState.IsPlaying = false;
    }

    public bool IsPlaying()
    {
        return _curState.IsPlaying;
    }

    public bool IsPlayFinish()
    {
        return _curState.Length - _curState.Time < UmUtil.GetOnFrameTime();
    }

    public void UpdateStateTime(float normTime)
    {
        _curState.NormalizedTime = normTime;
    }

    public void PlayFinish()
    {
        PauseAnim();
        _action.GoToEndFrame();
    }

    public float GetAnimRate()
    {
        return _action.GetCurPlayRate();
    }
    
    public bool IsAnimRateFinish()
    {
        return _action.IsAnimationFinish();
    }

    public void SetActionStartRate(float argRate)
    {
        _action.SetStartRate(argRate);
    }
    
    public void SetActionEndRate(float argRate)
    {
        _action.SetEndRate(argRate);
    }
}