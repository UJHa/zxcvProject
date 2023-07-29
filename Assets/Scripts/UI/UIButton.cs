using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIButton : Button
    {
        private TextMeshProUGUI textField;

        // Button의 Awake는 Edit/Play 시점 모두 호출된다.
        protected override void Awake()
        {
            base.Awake();
            Debug.Log($"[UIButton]Awake call! name({name})");
            if (transform.Find("Text (TMP)").TryGetComponent<TextMeshProUGUI>(out var textObj))
                textField = textObj;
        }

        public void SetText(string argText)
        {
            if (null == textField)
                return;
            textField.text = argText;
        }
    }
}