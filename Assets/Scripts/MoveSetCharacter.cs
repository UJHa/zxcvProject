using System;
using Animancer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 엄todo: 기능 개발 요구사항
// 캐릭터 회전이 가능하도록 수정하기
// 충돌체 붙일 수 있도록 세팅 가져오기
// 캐릭터의 로직에 있는 데이터를 상위 Manager로 관리하기
public class MoveSetCharacter : MonoBehaviour
{
    private int testFrame = 120;
    private float oneFrameTime;
    private AnimancerComponent _animancer = null;
    private AnimationClip _curClip = null;
    private AnimancerState _curState = null;
    public Slider _slider;
    public TextMeshProUGUI _curStateTxt;
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
        _curClip = Resources.Load<AnimationClip>("Animation/Lucy_FightFist01_1");
        oneFrameTime = 1f / testFrame;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _curState.Time = 0f;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _curState = _animancer.Play(_curClip);
            _curState.Time = 0f;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _curState.IsPlaying = !_curState.IsPlaying;
        }

        UpdateSlider();
        UpdateText();
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
            if (_curState.IsPlayingAndNotEnding())
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
            if (_curState.IsPlayingAndNotEnding())
                _slider.value = _curState.NormalizedTime;
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