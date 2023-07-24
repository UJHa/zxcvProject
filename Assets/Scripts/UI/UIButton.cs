using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIButton : Button
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(string argText)
        {
            _text.text = argText;
        }
    }
}