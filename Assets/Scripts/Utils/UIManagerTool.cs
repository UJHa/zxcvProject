using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Utils
{
    public class UIManagerTool : UIManager
    {
        private static UIManagerTool instance = null;
        public static UIManagerTool Instance => instance;
        // MoveSetScene용 UI 페이지
        public UIActionPlayerPage actionPlayerPage;
        public UIContextMenuPopup contextMenuPopup;
        public UILayoutWindow animCustomWindow;
        
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
                    contextMenuPopup?.Hide();
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
                            contextMenuPopup.SetupMenus(menuList);
                            Debug.Log($"[testum]Raycast result on");
                            contextMenuPopup.Show();
                        }
                    }
                }
            }
        }
        
        public List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

        public bool IsRaycastUI()
        {
            var results = GetEventSystemRaycastResults();
            return results.Count > 0;
        }
    }
}