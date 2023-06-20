using System;
using Animancer;
using UnityEngine;

public class MoveSetCharacter : MonoBehaviour
{
    private AnimancerComponent _animancer;
    private AnimationClip _curClip;
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
        _curClip = Resources.Load<AnimationClip>("Animation/Lucy_FightFist01_1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            _animancer.Play(_curClip);
    }
}