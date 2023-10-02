using TMPro;
using UnityEngine;

namespace UI
{
    public class UITextInputField : UIWidget
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TMP_InputField _inputField;

        public void SetGuideText(string argText)
        {
            _text.text = argText;
        }
        
        public void SetPlaceHolder(string argText)
        {
            _inputField.placeholder.GetComponent<TextMeshProUGUI>().text = argText;
        }
        
        public void SetInputField(string argText)
        {
            _inputField.text = argText;
        }

        public string GetFieldData()
        {
            return _inputField.text;
        }
    }
}