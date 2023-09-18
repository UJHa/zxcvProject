using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public partial class UIManager
    {
        
    }
}

namespace Utils
{
    public enum AnchorPresetType
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        MIDDLE_LEFT,
        MIDDLE_CENTER,
        MIDDLE_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,
        VERT_STRETCH_LEFT,
        VERT_STRETCH_CENTER,
        VERT_STRETCH_RIGHT,
        HOR_STRETCH_TOP,
        HOR_STRETCH_MIDDLE,
        HOR_STRETCH_BOTTOM,
        STRETCH_ALL
    }

    public class AnchorVector
    {
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
    }
    
    public enum UILayerType
    {
        LayerNormal,
        LayerPopup,
        
    }
    public partial class UIManager : MonoBehaviour
    {
        private Dictionary<UILayerType, GameObject> _layerParents = new();
        private Dictionary<string, GameObject> _uiPrefabs = new();
        private Dictionary<AnchorPresetType, AnchorVector> _anchorVectors = new();
        
        private void Awake()
        {
            InitInstance();
            Debug.Log($"uimanager awake");
            foreach (Transform tfm in transform)
            {
                Debug.Log($"obj.name({tfm.name})");
                if (Enum.TryParse<UILayerType>(tfm.name, out var uiLayerType))
                    _layerParents[uiLayerType] = tfm.gameObject;
            }

            InitAnchors();
        }

        protected virtual void InitInstance()
        {
            
        }

        private void InitAnchors()
        {
            _anchorVectors.Add(AnchorPresetType.TOP_LEFT, new()
            {
                AnchorMin = new(0f,1f),
                AnchorMax = new(0f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.TOP_CENTER, new()
            {
                AnchorMin = new(0.5f,1f),
                AnchorMax = new(0.5f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.TOP_RIGHT, new()
            {
                AnchorMin = new(1f,1f),
                AnchorMax = new(1f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.MIDDLE_LEFT, new()
            {
                AnchorMin = new(0f,0.5f),
                AnchorMax = new(0f,0.5f)
            });
            _anchorVectors.Add(AnchorPresetType.MIDDLE_CENTER, new()
            {
                AnchorMin = new(0.5f,0.5f),
                AnchorMax = new(0.5f,0.5f)
            });
            _anchorVectors.Add(AnchorPresetType.MIDDLE_RIGHT, new()
            {
                AnchorMin = new(1f,0.5f),
                AnchorMax = new(1f,0.5f)
            });
            _anchorVectors.Add(AnchorPresetType.BOTTOM_LEFT, new()
            {
                AnchorMin = new(0f,0f),
                AnchorMax = new(0f,0f)
            });
            _anchorVectors.Add(AnchorPresetType.BOTTOM_CENTER, new()
            {
                AnchorMin = new(0.5f,0f),
                AnchorMax = new(0.5f,0f)
            });
            _anchorVectors.Add(AnchorPresetType.BOTTOM_RIGHT, new()
            {
                AnchorMin = new(1f,0f),
                AnchorMax = new(1f,0f)
            });
            _anchorVectors.Add(AnchorPresetType.VERT_STRETCH_LEFT, new()
            {
                AnchorMin = new(0f,0f),
                AnchorMax = new(0f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.VERT_STRETCH_CENTER, new()
            {
                AnchorMin = new(0.5f,0f),
                AnchorMax = new(0.5f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.VERT_STRETCH_RIGHT, new()
            {
                AnchorMin = new(1f,0f),
                AnchorMax = new(1f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.HOR_STRETCH_TOP, new()
            {
                AnchorMin = new(0f,1f),
                AnchorMax = new(1f,1f)
            });
            _anchorVectors.Add(AnchorPresetType.HOR_STRETCH_MIDDLE, new()
            {
                AnchorMin = new(0f,0.5f),
                AnchorMax = new(1f,0.5f)
            });
            _anchorVectors.Add(AnchorPresetType.HOR_STRETCH_BOTTOM, new()
            {
                AnchorMin = new(0f,0f),
                AnchorMax = new(1f,0f)
            });
            _anchorVectors.Add(AnchorPresetType.STRETCH_ALL, new()
            {
                AnchorMin = new(0f,0f),
                AnchorMax = new(1f,1f)
            });
        }

        public T CreateUI<T>(string prefabPath, UILayerType layerType)
        {
            if (false == _layerParents.ContainsKey(layerType))
            {
                Debug.LogError($"CreateUI failed! Not Contain UILayerType({layerType})");
                return default;
            }

            GameObject uiObj = null;
            if (_uiPrefabs.ContainsKey(prefabPath))
            {
                uiObj = _uiPrefabs[prefabPath];
            }
            else
            {
                var uiPrefab = Resources.Load<GameObject>(prefabPath);
                uiObj = Instantiate(uiPrefab, _layerParents[layerType].transform);
                _uiPrefabs.Add(prefabPath, uiObj);
            }
            
            if (false == uiObj.TryGetComponent<T>(out T ui))
            {
                Debug.LogError($"CreateUI failed! Not Contain UILayerType({layerType})");
                return default;
            }

            return ui;
        }
        
        public AnchorVector GetAnchorVector(AnchorPresetType presetType)
        {
            return _anchorVectors[presetType];
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