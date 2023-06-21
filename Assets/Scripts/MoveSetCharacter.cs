using System;
using Animancer;
using UnityEngine;
using UnityEngine.UI;

public class MoveSetCharacter : MonoBehaviour
{
    private AnimancerComponent _animancer = null;
    private AnimationClip _curClip = null;
    private AnimancerState _curState = null;
    public Slider _slider;
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
        _curClip = Resources.Load<AnimationClip>("Animation/Lucy_FightFist01_1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _curState = _animancer.Play(_curClip);
            _curState.Time = 0f;
        }
        if (null != _slider && null != _curState)
            _slider.value = _curState.NormalizedTime;
    }
}