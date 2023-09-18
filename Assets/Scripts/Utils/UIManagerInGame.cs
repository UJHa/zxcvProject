using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class UIManagerInGame : UIManager
    {
        private static UIManagerInGame instance = null;
        public static UIManagerInGame Instance => instance;
        // DevScene용 UI 페이지
        public UIHudManager hudManager;
        
        protected override void InitInstance()
        {
            if (instance)
            {
                Destroy(this.gameObject);
            }

            instance = this;
        }

        private void Update()
        {
            UpdateClick();
        }
        
        private void UpdateClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (false == IsRaycastUI())
                {
                    // contextMenuPopup?.Hide();
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                var results = GetEventSystemRaycastResults();
                if (results.Count > 0)
                {
                    if (results[0].gameObject.TryGetComponent<UIWidget>(out var widget))
                    {
                        var obj = widget.GetOwner();
                        if (obj.TryGetComponent<UISlider>(out var slider))
                        {
                            List<MenuAction> menuList = slider.GetMenuList();
                            // contextMenuPopup.SetupMenus(menuList);
                            // Debug.Log($"[testum]Raycast result on");
                            // contextMenuPopup.Show();
                        }
                    }
                }
            }
        }
    }
}