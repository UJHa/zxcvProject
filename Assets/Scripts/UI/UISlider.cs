using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public struct MenuAction
{
    public string btnName;
    public System.Action btnAction;
}

namespace UI
{
    public class UISlider : MonoBehaviour
    {
        private Slider _slider;
        private UIWidget _background;
        
        private List<MenuAction> _menuList = new();

        private void Awake()
        {
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                if (image.raycastTarget)
                {
                    if (image.TryGetComponent<UIWidget>(out var widget))
                        widget.SetOwner(gameObject);
                }
            }
        }
        
        public void Init(float minValue, float maxValue)
        {
            _slider = GetComponent<Slider>();
            
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.value = minValue;
            
            _menuList.Add(new MenuAction
            {
                btnName = "Start Pin",
                btnAction = UIManager.Instance.animCustomWindow.StartPin
            });
            _menuList.Add(new MenuAction
            {
                btnName = "End Pin",
                btnAction = UIManager.Instance.animCustomWindow.EndPin
            });
        }

        public void Test()
        {
            
        }

        public List<MenuAction> GetMenuList()
        {
            return _menuList;
        }

        public void SetValue(float argValue)
        {
            _slider.value = argValue;
        }
        
        public float GetValue()
        {
            return _slider.value;
        }

        public float GetMaxValue()
        {
            return _slider.maxValue;
        }
    }
}