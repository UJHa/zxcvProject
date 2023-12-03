using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIButton : Button
    {
        private TextMeshProUGUI _textField;

        // Button의 Awake는 Edit/Play 시점 모두 호출된다.
        protected override void Awake()
        {
            base.Awake();
            ReleaseLog.LogInfo($"[UIButton]Awake call! name({name})");
            if (transform.Find("Text (TMP)").TryGetComponent<TextMeshProUGUI>(out var textObj))
                _textField = textObj;
        }

        public void Init()
        {
            if (transform.Find("Text (TMP)").TryGetComponent<TextMeshProUGUI>(out var textObj))
                _textField = textObj;
        }

        public void SetText(string argText)
        {
            _textField.text = argText;
        }
    }
}