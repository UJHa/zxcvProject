using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIButton : Button
    {
        [SerializeField] private TextMeshProUGUI _text;

        protected override void Awake()
        {
            base.Awake();
            if (transform.Find("Text (TMP)").TryGetComponent<TextMeshProUGUI>(out var textObj))
                _text = textObj;
        }

        public void SetText(string argText)
        {
            if (null == _text)
                return;
            _text.text = argText;
        }
    }
}