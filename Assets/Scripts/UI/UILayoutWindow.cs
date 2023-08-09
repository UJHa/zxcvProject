using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UILayoutWindow : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private ScrollRect _scrollRect;
        public void Init()
        {
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
            var animSlider = Instantiate(sliderObj, _scrollRect.content);
        }
    }
}