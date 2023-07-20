using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public enum UILayerType
    {
        LayerNormal,
        LayerPopup,
        
    }
    public class UIManager : MonoBehaviour
    {
        private Dictionary<UILayerType, GameObject> _layerParents = new();
        private void Awake()
        {
            Debug.Log($"uimanager awake");
            foreach (Transform tfm in transform)
            {
                Debug.Log($"obj.name({tfm.name})");
                if (Enum.TryParse<UILayerType>(tfm.name, out var uiLayerType))
                    _layerParents[uiLayerType] = tfm.gameObject;
            }
        }

        public void CreateUI(string prefabPath, UILayerType layerType)
        {
            var uiPrefab = Resources.Load<GameObject>(prefabPath);
            if (false == _layerParents.ContainsKey(layerType))
            {
                Debug.LogError($"CreateUI failed! Not Contain UILayerType({layerType})");
                return;
            }

            Instantiate(uiPrefab, _layerParents[layerType].transform);
        }
    }
}