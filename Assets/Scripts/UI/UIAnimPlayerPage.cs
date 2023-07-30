using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public enum AnimEditState
{
    Play,
    Stop
}

namespace UI
{
    public class UIAnimPlayerPage : MonoBehaviour
    {
        // 엄todo : 나중에 Resources나 특정 디렉토리의 파일 리스트를 가져오도록 개발 필요
        [Header("Test Anim Datas")]
        [SerializeField] private List<string> _animInfos = new();
        [Header("UI State")]
        [SerializeField] private AnimEditState _animEditState;
        [Header("UI object")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private UIButton _infoBtnPrefab;
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _curStateTxt;
        [SerializeField] private Button _play;

        // 엄todo UI와 연결 의존도 낮추기 위해 제거되어야 한다.
        private MoveSetCharacter _moveSetCharacter;
        
        public void Init(MoveSetCharacter moveSetCharacter, float minValue, float maxValue)
        {
            _moveSetCharacter = moveSetCharacter;
            var prefabName = _infoBtnPrefab.name;
            foreach (var animInfo in _animInfos)
            {
                Debug.Log($"[testum]animInfo({animInfo})");
                _infoBtnPrefab.name = animInfo;
                var btnObj = Instantiate(_infoBtnPrefab, _scrollRect.content);
                btnObj.SetText(animInfo);
                btnObj.onClick.AddListener(() =>
                {
                    Debug.Log($"[testum]Click button name({btnObj.name})");
                    _moveSetCharacter.ChangeAction(animInfo, 0f, 1f);
                });
            }

            _infoBtnPrefab.name = prefabName;

            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.value = minValue;

            if (_slider.TryGetComponent<EventTrigger>(out var trigger))
            {
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(SliderOnPointerDown);
                    trigger.triggers.Add(entry);
                }
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerUp;
                    entry.callback.AddListener(SliderOnPointerUp);
                    trigger.triggers.Add(entry);
                }
            }
            
            _play.onClick.AddListener(() =>
            {
                _moveSetCharacter.PlayAnim();
                SetAnimEditState(AnimEditState.Play);
            });
        }
        
        private void Update()
        {
            UpdateSlider();
            UpdateText();
        }

        private void UpdateSlider()
        {
            if (null == _slider)
                return;
            if (_moveSetCharacter.IsPlayFinish())
            {
                SetAnimEditState(AnimEditState.Stop);
                if (false == _moveSetCharacter.IsAnimRateFinish())
                {
                    _slider.value = _slider.maxValue;
                    _moveSetCharacter.PlayFinish();
                }
                else
                    _moveSetCharacter.UpdateStateTime(_slider.value);
                // Debug.Log($"[testum]log1 _curNormValue({_curState.NormalizedTime}) curTimeValue({_curState.Time}) totalTime({_curState.Length})");
            }
            else
            {
                if (false == _moveSetCharacter.IsAnimRateFinish() && _moveSetCharacter.IsPlaying())
                    _slider.value = _moveSetCharacter.GetAnimRate();
                else
                    _moveSetCharacter.UpdateStateTime(_slider.value);

                // Debug.Log($"[testum]log2 _curNormValue({_curState.NormalizedTime}) curTimeValue({_curState.Time}) totalTime({_curState.Length}) test({_curState.Length - _curState.Time})");
            }
        }
        
        private void UpdateText()
        {
            if (null == _curStateTxt)
                return;
            _curStateTxt.text = _animEditState.ToString();
        }

        public void SetAnimEditState(AnimEditState editState)
        {
            _animEditState = editState;
        }

        private void OnDestroy()
        {
            _play.onClick.RemoveAllListeners();
        }

        private void SliderOnPointerDown(BaseEventData argData)
        {
            Debug.Log($"[testum]SliderOnPointerDown");
            UmUtil.SetSliderHold(true);
        }
    
        private void SliderOnPointerUp(BaseEventData argData)
        {
            Debug.Log($"[testum]SliderOnPointerUp");
            UmUtil.SetSliderHold(false);
        }
    }
}