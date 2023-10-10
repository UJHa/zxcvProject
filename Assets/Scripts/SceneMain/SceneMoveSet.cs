using DataClass;
using UI;
using UnityEngine;
using Utils;

namespace SceneMain
{
    public class SceneMoveSet : MonoBehaviour
    {
        private int testFrame = 120;
        private float oneFrameTime;
        
        private Canvas _canvas = null;
        private UIManagerTool _uiManager = null;
        private MoveSetCharacter _moveSetCharacter;
        private void Awake()
        {
            DataTable.LoadJsonData();
            
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
            
            _moveSetCharacter = CreateCharacter();

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
            _uiManager.actionPlayerPage = _uiManager.CreateUI<UIActionPlayerPage>("Prefabs/UI/ActionPlayerPage", UILayerType.LayerNormal);
            _uiManager.actionPlayerPage.Init(_moveSetCharacter);
            
            _uiManager.contextMenuPopup = _uiManager.CreateUI<UIContextMenuPopup>("Prefabs/UI/Common/ContextMenuPopup", UILayerType.LayerPopup);
            _uiManager.contextMenuPopup.Init();

            _uiManager.actionPlayerPage.transform.SetAsLastSibling();
        }

        private UIManagerTool LoadUIManager()
        {
            var loadPrefab = Resources.Load<GameObject>("Prefabs/UIManagerTool");
            if (null == loadPrefab)
            {
                Debug.LogError($"[testum]uiManager({loadPrefab}) fail");
                return null;
            }
        
            Debug.Log($"[testum]uiManager({loadPrefab}) success");
            var uiManagerObj = Instantiate(loadPrefab, _canvas.transform);
            if (null == uiManagerObj)
                return null;
            if (false == uiManagerObj.TryGetComponent<UIManagerTool>(out var uiMgr))
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