using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

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
    }
}