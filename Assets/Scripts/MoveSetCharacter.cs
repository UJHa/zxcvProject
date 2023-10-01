using System;
using Animancer;
using DataClass;
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
        // ChangeAction("Animation/Lucy_FightFist01_2", animStartTime, animEndTime);
        ChangeAction(ActionTable.GetActionData(eState.IDLE.ToString()));
    }

    // ChangeAction이 가지고 있는 액션일 때는 json 로드로 생성하는 기능 구현하기
    public void ChangeAction(ActionData actionData)
    {
        _action = new Action(_animancer, actionData.actionName);
        _action.Init();
        _curState = _action.Play();
        _action.SetNormTime(actionData.startTimeRatio);
        PauseAnim();
    }

    private float _startMousePosX = 0f;
    private Vector3 _startRot = new();

    private void Update()
    {
        if (UIManagerTool.Instance.IsRaycastUI())
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
        if (IsAnimRateFinish())
            _action.GoToFirstFrame();
        _curState = _action.Play();
    }

    public void PlayPinAnim()
    {
        if (IsAnimRateFinish())
            _action.GoToFirstFrame();
        _curState = _action.Play();
    }

    public void PauseAnim()
    {
        _curState.IsPlaying = false;
    }

    public bool IsPlaying()
    {
        if (null == _action || null == _curState)
            return false;
        return _curState.IsPlaying;
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
    
    public float GetStartAnimRate()
    {
        if (null == _action)
            return 0f;
        return _action.GetStartRate();
    }
    
    public float GetEndAnimRate()
    {
        if (null == _action)
            return 1f;
        return _action.GetEndRate();
    }
    
    public bool IsAnimRateFinish()
    {
        return _action.IsAnimationFinish();
    }

    public void ExportCurAction()
    {
        Debug.Log("Export!");
        _action.Export();
    }

    public void SetActionStartRate(float argRate)
    {
        _action.SetNormTime(argRate);
    }
    
    public void SetActionEndRate(float argRate)
    {
        _action.SetEndRate(argRate);
    }
}