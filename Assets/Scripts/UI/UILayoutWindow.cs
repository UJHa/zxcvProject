using UnityEngine;
using Utils;

namespace UI
{
    public class UILayoutWindow : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public void Init()
        {
            if (TryGetComponent<RectTransform>(out var rectTransform))
                _rectTransform = rectTransform;

            // window 구성 세팅 작업
            var bottomCenter = UIManager.Instance.GetAnchorVector(AnchorPresetType.BOTTOM_CENTER);
            _rectTransform.anchorMin = bottomCenter.AnchorMin;
            _rectTransform.anchorMax = bottomCenter.AnchorMax;
            _rectTransform.sizeDelta = new(1920, 500);
        }
    }
}