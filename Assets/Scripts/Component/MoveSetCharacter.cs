using Animancer;
using DataClass;
using UnityEngine;
using Utils;

public class MoveSetCharacter : MonoBehaviour
{
    private AnimancerState _curState = null;

    private AnimancerComponent _animancer;
    private Action _action = null;
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
    }

    // ChangeAction이 가지고 있는 액션일 때는 json 로드로 생성하는 기능 구현하기
    public void ChangeAction(ActionData actionData)
    {
        _action = new Action(actionData.name);
        _action.Init();
        _curState = Play(_action);
        PauseAnim();
    }

    public AnimancerState Play(Action action)
    {
        var result = _animancer.Play(action.GetClip());
        result.NormalizedTime = _action.GetStartRatio();
        result.Speed = _action.GetSpeed();
        return result;
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
            GoToFirstFrame();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _curState.IsPlaying = !_curState.IsPlaying;
        }
    }
    
    public void PlayAnim(ActionData actionData)
    {
        _action.SetActionData(actionData);
        if (IsAnimRateFinish())
            GoToFirstFrame();
        _curState = Play(_action);
        Debug.Log($"[testTime]time({this._curState.Length})");
    }

    public void PlayPinAnim()
    {
        if (IsAnimRateFinish())
            GoToFirstFrame();
        _curState = Play(_action);
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
        GoToEndFrame();
    }

    public float GetAnimRate()
    {
        return _curState.NormalizedTime;
    }
    
    public float GetStartAnimRate()
    {
        if (null == _action)
            return 0f;
        return _action.GetStartRatio();
    }
    
    public float GetEndAnimRate()
    {
        if (null == _action)
            return 1f;
        return _action.GetEndRatio();
    }
    
    public bool IsAnimRateFinish()
    {
        return _curState.NormalizedTime >= _action.GetEndRatio();
    }

    public void SetActionStartRate(float argRate)
    {
        _curState.NormalizedTime = argRate;
    }
    
    public void SetActionEndRate(float argRate)
    {
        _action.SetEndRate(argRate);
    }
    
    void GoToFirstFrame()
    {
        _curState.NormalizedTime = _action.GetStartRatio();
    }
    
    void GoToEndFrame()
    {
        _curState.NormalizedTime = _action.GetEndRatio();
    }
}