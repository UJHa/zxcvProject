using UnityEngine;
using Utils;

namespace SceneMain
{
    public class SceneMoveSet : MonoBehaviour
    {
        private Canvas _canvas = null;
        private UIManager _uiManager = null;
        private void Awake()
        {
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
            _uiManager.CreateUI("Prefabs/UI/AnimPlayerPage", UILayerType.LayerNormal);
            
            // Character 명세별 생성(일단 1개)
            var moveSetCharacter = CreateCharacter();
        }

        private UIManager LoadUIManager()
        {
            var loadPrefab = Resources.Load<GameObject>("Prefabs/UIManager");
            if (null == loadPrefab)
            {
                Debug.LogError($"[testum]uiManager({loadPrefab}) fail");
                return null;
            }
        
            Debug.Log($"[testum]uiManager({loadPrefab}) success");
            var uiManagerObj = Instantiate(loadPrefab, _canvas.transform);
            if (null == uiManagerObj)
                return null;
            if (false == uiManagerObj.TryGetComponent<UIManager>(out var uiMgr))
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