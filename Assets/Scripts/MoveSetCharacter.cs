using System;
using Animancer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 엄todo: 기능 개발 요구사항
// 캐릭터 회전이 가능하도록 수정하기
// >> 추가 작업 : slider와 같은 UI는 추가 레이어 관리 UIManager 만들기(마우스 홀드와 UI가 겹칠때 처리)
// 캐릭터 액션 기반 처리하기
// >> 추가 작업 : Slider min,max를 action 객체가 가진 start,end 비율로 변경
// >> 추가 노가다 작업 : 애니메이션 시작, 종료 시간은 에디팅때만 제어하도록 만들기 위해서 스크립트로 애니메이션 자르기 제어 가능한지 확인하기
// 충돌체 붙일 수 있도록 세팅 가져오기
// 캐릭터의 로직에 있는 데이터를 상위 Manager로 관리하기
public class MoveSetCharacter : MonoBehaviour
{
    private int testFrame = 120;
    private float oneFrameTime;
    private AnimancerState _curState = null;
    public Slider _slider;
    public TextMeshProUGUI _curStateTxt;
    private float _minValue = 0.0f;
    private float _maxValue = 1f;
    
    private MoveSet _moveSet = new();
    private Action _action = null;

    private bool _isSliderHold = false;
    private void Awake()
    {
        oneFrameTime = 1f / testFrame;
        
        _moveSet.Init(gameObject);
        // _moveSet.RegisterAction(eState.ATTACK, KeyCode.C, eState.IDLE, new ActionInfo("Animation/Lucy_FightFist01_1", 0f, 0.7f, AttackRangeType.PUNCH_A, 0.15f, 0.4f, new(AttackType.NORMAL, 0.1f , 0.1f)));
        _moveSet.RegisterAction(eState.ATTACK, KeyCode.C, eState.ATTACK, new ActionInfo("Animation/Lucy_FightFist01_2", _minValue, _maxValue, AttackRangeType.PUNCH_A, 0.0f, 0.3f, new(AttackType.NORMAL, 0.1f, 0.1f)));
        _slider.minValue = _minValue;
        _slider.maxValue = _maxValue;
        _action = _moveSet.GetAction(eState.ATTACK);
        _slider.value = _minValue;
    }

    private float _startMousePosX = 0f;
    private Vector3 _startRot = new();

    private void Update()
    {
        if (false == _isSliderHold)
        {
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
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _action.Reset();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (null != _curState)
                _action.Reset();
            _curState = _moveSet.Play(eState.ATTACK);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _curState.IsPlaying = !_curState.IsPlaying;
        }

        UpdateSlider();
        UpdateText();
    }

    public void SliderOnPointerDown()
    {
        Debug.Log($"[testum]SliderOnPointerDown");
        _isSliderHold = true;
    }
    
    public void SliderOnPointerUp()
    {
        Debug.Log($"[testum]SliderOnPointerUp");
        _isSliderHold = false;
    }

    // 뇌 정리되면 여기 코드 개선 필요
    private void UpdateSlider()
    {
        if (null == _curState)
            return;
        if (null == _slider)
            return;
        if (_curState.Length - _curState.Time < oneFrameTime)
        {
            if (false == _action.IsAnimationFinish())
            {
                _slider.value = _slider.maxValue;
                _curState.IsPlaying = false;
                _curState.NormalizedTime = _slider.maxValue;
            }
            else
                _curState.NormalizedTime = _slider.value;
            // Debug.Log($"[testum]log1 _curNormValue({_curState.NormalizedTime}) curTimeValue({_curState.Time}) totalTime({_curState.Length})");
        }
        else
        {
            if (false == _action.IsAnimationFinish() && _curState.IsPlaying)
                _slider.value = (_curState.NormalizedTime - _minValue) / (_maxValue - _minValue);
            else
                _curState.NormalizedTime = _slider.value;
            
            // Debug.Log($"[testum]log2 _curNormValue({_curState.NormalizedTime}) curTimeValue({_curState.Time}) totalTime({_curState.Length}) test({_curState.Length - _curState.Time})");
        }
    }

    private void UpdateText()
    {
        if (null == _curState)
            return;
        if (null == _curStateTxt)
            return;
        if (_curState.IsPlayingAndNotEnding())
            _curStateTxt.text = "IsPlayingAndNotEnding";
        else
            _curStateTxt.text = "IsNone";
    }
}