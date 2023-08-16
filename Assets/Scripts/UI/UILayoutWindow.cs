using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UILayoutWindow : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private ScrollRect _scrollRect;

        private MoveSetCharacter _moveSetCharacter;
        [Header("Anim customize UIs")]
        [SerializeField] private UISlider _slider;
        public void Init(MoveSetCharacter moveSetCharacter)
        {
            _moveSetCharacter = moveSetCharacter;
            if (TryGetComponent<RectTransform>(out var rectTransform))
                _rectTransform = rectTransform;

            var scroll = GetComponentInChildren<ScrollRect>();
            if (null != scroll)
            {
                _scrollRect = scroll;
                Debug.Log($"[testum]scroll rect find!");
            }

            // window 구성 세팅 작업
            var bottomCenter = UIManager.Instance.GetAnchorVector(AnchorPresetType.BOTTOM_CENTER);
            _rectTransform.anchorMin = bottomCenter.AnchorMin;
            _rectTransform.anchorMax = bottomCenter.AnchorMax;
            _rectTransform.sizeDelta = new(1920, 300);
            _rectTransform.pivot = new(0.5f, 0f);
            var sliderObj = Resources.Load<UISlider>("Prefabs/UI/Common/UISlider");
            if (sliderObj.TryGetComponent<RectTransform>(out var sliderRect))
            {
                var bottomStretch = UIManager.Instance.GetAnchorVector(AnchorPresetType.HOR_STRETCH_BOTTOM);
                sliderRect.anchorMin = bottomStretch.AnchorMin;
                sliderRect.anchorMax = bottomStretch.AnchorMax;
            }
            _slider = Instantiate(sliderObj, _scrollRect.content);
            _slider.Init(moveSetCharacter.GetStartAnimRate(), moveSetCharacter.GetEndAnimRate());

            if (_slider.TryGetComponent<EventTrigger>(out var trigger))
            {
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(UmUtil.SliderOnPointerDown);
                    trigger.triggers.Add(entry);
                }
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerUp;
                    entry.callback.AddListener(UmUtil.SliderOnPointerUp);
                    trigger.triggers.Add(entry);
                }
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener(UmUtil.SliderOnPointerClick);
                    trigger.triggers.Add(entry);
                }
            }
        }

        private void Update()
        {
            UpdateSlider();
        }
        
        private void UpdateSlider()
        {
            if (null == _slider)
                return;
            if (UmUtil.IsSliderHold())
            {
                // SetAnimEditState(AnimEditState.Stop);
                _moveSetCharacter.PauseAnim();
                _moveSetCharacter.UpdateStateTime(_slider.GetValue());
            }
            else if (_moveSetCharacter.IsPlaying())
            {
                _slider.SetValue(_moveSetCharacter.GetAnimRate());
                if (_moveSetCharacter.IsAnimRateFinish())
                {
                    // SetAnimEditState(AnimEditState.Stop);
                    _slider.SetValue(_moveSetCharacter.GetEndAnimRate());
                    _moveSetCharacter.PlayFinish();
                }
            }
        }
        
        public void StartPin()
        {
            var pinStart = UIManager.Instance.CreateUI<Image>("Prefabs/UI/PinStart", UILayerType.LayerPopup);
            
            if (pinStart.TryGetComponent<RectTransform>(out var rfm))
            {
                rfm.anchoredPosition = UIManager.Instance.contextMenuPopup.transform.position;
            }
            UIManager.Instance.contextMenuPopup.Hide();
            var startRate = UIManager.Instance.contextMenuPopup.GetPositionToSliderRate(_slider, rfm.anchoredPosition.x);
            Debug.Log($"[startpin] position({pinStart.transform.position}) rectPos({rfm.position}) anPos({rfm.anchoredPosition})");
            Debug.Log($"[startpin] startRate({startRate})");
            // _startRate = GetPositionToSliderRate(_slider, rfm.anchoredPosition.x);
            _moveSetCharacter.SetActionStartRate(startRate);
        }

        public void EndPin()
        {
            var pinEnd = UIManager.Instance.CreateUI<Image>("Prefabs/UI/PinEnd", UILayerType.LayerPopup);
            
            if (pinEnd.TryGetComponent<RectTransform>(out var rfm))
            {
                rfm.anchoredPosition = UIManager.Instance.contextMenuPopup.transform.position;
            }
            UIManager.Instance.contextMenuPopup.Hide();
            var endRate = UIManager.Instance.contextMenuPopup.GetPositionToSliderRate(_slider, rfm.anchoredPosition.x);
            Debug.Log($"[endpin] endRate({endRate})");
            // _endRate = GetPositionToSliderRate(_slider, rfm.anchoredPosition.x);
            _moveSetCharacter.SetActionEndRate(endRate);
        }
    }
}