using System;
using Animancer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveSetCharacter : MonoBehaviour
{
    private AnimancerComponent _animancer = null;
    private AnimationClip _curClip = null;
    private AnimancerState _curState = null;
    public Slider _slider;
    public TextMeshProUGUI _curStateTxt;
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
        _curClip = Resources.Load<AnimationClip>("Animation/Lucy_FightFist01_1");
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
            _curState.IsPlaying = false;
        }

        UpdateSlider();
        UpdateText();
    }

    private void UpdateSlider()
    {
        if (null == _curState)
            return;
        if (null == _slider)
            return;
        if (_curState.IsPlayingAndNotEnding())
            _slider.value = _curState.NormalizedTime;
        else
            _curState.NormalizedTime = _slider.value;
    }

    private void UpdateText()
    {
        if (null == _curState)
            return;
        if (null == _curStateTxt)
            return;
        if (_curState.IsStopped)
            _curStateTxt.text = "IsStopped";
        else if (_curState.IsLooping)
            _curStateTxt.text = "IsLooping";
        else if (_curState.IsPlayingAndNotEnding())
            _curStateTxt.text = "IsPlayingAndNotEnding";
        else
            _curStateTxt.text = "IsNone";
    }
}