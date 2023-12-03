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
    public class SceneStage : MonoBehaviour
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
                if (canvasObj.TryGetComponent<Canvas>(out var canvas))
                {
                    _canvas = canvas;
                }
            }

            _uiManager = LoadUIManager();
            if (null != _uiManager)
            {
                ReleaseLog.LogInfo($"[testum]uiManager({_uiManager}) success");
            }
            else
            {
                ReleaseLog.LogInfo($"[testum]uiManager({_uiManager}) fail");
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
                ReleaseLog.LogError($"[testum]uiManager({loadPrefab}) fail");
                return null;
            }
            
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