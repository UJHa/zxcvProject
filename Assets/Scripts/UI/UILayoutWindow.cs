using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UILayoutWindow : MonoBehaviour
    {
        private UIManagerTool _uiManager;
        private RectTransform _rectTransform;
        private ScrollRect _scrollRect;

        private MoveSetCharacter _moveSetCharacter;
        [Header("Anim customize UIs")]
        [SerializeField] private UISlider _slider;
        public void Init(MoveSetCharacter moveSetCharacter)
        {
            _uiManager = UIManagerTool.Instance;
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
            var bottomCenter = UIManagerTool.Instance.GetAnchorVector(AnchorPresetType.BOTTOM_CENTER);
            _rectTransform.anchorMin = bottomCenter.AnchorMin;
            _rectTransform.anchorMax = bottomCenter.AnchorMax;
            _rectTransform.sizeDelta = new(1920, 300);
            _rectTransform.pivot = new(0.5f, 0f);
            var sliderObj = Resources.Load<UISlider>("Prefabs/UI/Common/UISlider");
            if (sliderObj.TryGetComponent<RectTransform>(out var sliderRect))
            {
                var bottomStretch = UIManagerTool.Instance.GetAnchorVector(AnchorPresetType.HOR_STRETCH_BOTTOM);
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
                _moveSetCharacter.PauseAnim();
                _moveSetCharacter.UpdateStateTime(_slider.GetValue());
            }
            else if (_moveSetCharacter.IsPlaying())
            {
                _slider.SetValue(_moveSetCharacter.GetAnimRate());
                if (_moveSetCharacter.IsAnimRateFinish())
                {
                    _slider.SetValue(_moveSetCharacter.GetEndAnimRate());
                    _moveSetCharacter.PlayFinish();
                }
            }
        }
        
        public void StartPin()
        {
            var pinStart = _uiManager.CreateUI<Image>("Prefabs/UI/PinStart", UILayerType.LayerPopup);
            
            if (pinStart.TryGetComponent<RectTransform>(out var rfm))
            {
                rfm.anchoredPosition = _uiManager.contextMenuPopup.transform.position;
            }
            _uiManager.contextMenuPopup.Hide();
            var startRate = _uiManager.contextMenuPopup.GetPositionToSliderRate(_slider, rfm.anchoredPosition.x);
            Debug.Log($"[startpin] position({pinStart.transform.position}) rectPos({rfm.position}) anPos({rfm.anchoredPosition})");
            Debug.Log($"[startpin] startRate({startRate})");
            _moveSetCharacter.SetActionStartRate(startRate);
        }

        public void EndPin()
        {
            var pinEnd = _uiManager.CreateUI<Image>("Prefabs/UI/PinEnd", UILayerType.LayerPopup);
            
            if (pinEnd.TryGetComponent<RectTransform>(out var rfm))
            {
                rfm.anchoredPosition = _uiManager.contextMenuPopup.transform.position;
            }
            _uiManager.contextMenuPopup.Hide();
            var endRate = _uiManager.contextMenuPopup.GetPositionToSliderRate(_slider, rfm.anchoredPosition.x);
            Debug.Log($"[endpin] endRate({endRate})");
            _moveSetCharacter.SetActionEndRate(endRate);
        }
    }
}