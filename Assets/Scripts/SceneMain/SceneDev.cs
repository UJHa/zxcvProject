using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataClass;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using Utils;

namespace SceneMain
{
    public class SceneDev : MonoBehaviour
    {
        private Canvas _canvas = null;
        private UIManagerInGame _uiManager = null;
        private MoveSetCharacter _moveSetCharacter;
        private void Awake()
        {
            GameManager.CreateInstance();
            GameManager.Instance.Init();
            // GameManager.Instance.camera = Camera.main;

            var canvasObj = GameObject.Find("Canvas");
            if (null != canvasObj)
            {
                Debug.Log($"[testum]canvas obj name({canvasObj.name})");
                if (canvasObj.TryGetComponent<Canvas>(out var canvas))
                {
                    _canvas = canvas;
                    Debug.Log($"[testum]canvas find success!");
                }
            }

            _uiManager = LoadUIManager();
            if (null != _uiManager)
            {
                Debug.Log($"[testum]uiManager({_uiManager}) success");
            }
            else
            {
                Debug.Log($"[testum]uiManager({_uiManager}) fail");
            }
            // 필요 UI 명세별 생성
            _uiManager.hudManager = _uiManager.CreateUI<UIHudManager>("Prefabs/UI/HudManager", UILayerType.LayerNormal);
            _uiManager.hudManager.Init();
            
            var spawners = GameObject.FindObjectsOfType<Spawner>();
            foreach (var spawner in spawners)
            {
                spawner.SpawnObject();
            }

            GameManager.Instance.SetMainPlayer();
        }

        private UIManagerInGame LoadUIManager()
        {
            var loadPrefab = Resources.Load<GameObject>("Prefabs/UIManagerInGame");
            if (null == loadPrefab)
            {
                Debug.LogError($"[testum]uiManager({loadPrefab}) fail");
                return null;
            }
        
            Debug.Log($"[testum]uiManager({loadPrefab}) success");
            var uiManagerObj = Instantiate(loadPrefab, _canvas.transform);
            if (null == uiManagerObj)
                return null;
            if (false == uiManagerObj.TryGetComponent<UIManagerInGame>(out var uiMgr))
                return null;
        
            return uiMgr;
        }

        private MoveSetCharacter CreateCharacter()
        {
            var loadPrefab = Resources.Load<GameObject>("Prefabs/Character/MoveSetCharacter");
            var characterObj = Instantiate(loadPrefab);
            if (characterObj.TryGetComponent<MoveSetCharacter>(out var moveSetCharacter))
                return moveSetCharacter;
            else
                return null;
        }
    }
}