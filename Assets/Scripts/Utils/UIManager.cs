using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public partial class UIManager
    {
        public UIAnimPlayerPage animPlayerPage;
        public UIContextMenuPopup contextMenuPopup;
    }
}

namespace Utils
{
    public enum UILayerType
    {
        LayerNormal,
        LayerPopup,
        
    }
    public partial class UIManager : MonoBehaviour
    {
        private static UIManager instance = null;
        public static UIManager Instance => instance;
        private Dictionary<UILayerType, GameObject> _layerParents = new();
        
        
        private void Awake()
        {
            if (instance)
            {
                Destroy(this.gameObject);
            }

            instance = this;
            Debug.Log($"uimanager awake");
            foreach (Transform tfm in transform)
            {
                Debug.Log($"obj.name({tfm.name})");
                if (Enum.TryParse<UILayerType>(tfm.name, out var uiLayerType))
                    _layerParents[uiLayerType] = tfm.gameObject;
            }
        }

        public T CreateUI<T>(string prefabPath, UILayerType layerType)
        {
            var uiPrefab = Resources.Load<GameObject>(prefabPath);
            if (false == _layerParents.ContainsKey(layerType))
            {
                Debug.LogError($"CreateUI failed! Not Contain UILayerType({layerType})");
                return default;
            }

            var uiObj = Instantiate(uiPrefab, _layerParents[layerType].transform);
            if (false == uiObj.TryGetComponent<T>(out T ui))
            {
                Debug.LogError($"CreateUI failed! Not Contain UILayerType({layerType})");
                return default;
            }

            return ui;
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
                    contextMenuPopup.Hide();
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