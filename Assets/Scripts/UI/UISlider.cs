using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISlider : MonoBehaviour
    {
        private Slider _slider;
        private UIWidget _background;
        
        private List<string> _menuList = new();

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
            
            _menuList.Add("Start Pin");
            _menuList.Add("End Pin");
        }

        public void Test()
        {
            
        }

        public List<string> GetMenuList()
        {
            return _menuList;
        }
    }
}