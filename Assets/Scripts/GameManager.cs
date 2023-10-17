using System.Collections.Generic;
using DataClass;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    public static bool IsExistInstance = false;
    
    public static void CreateInstance()
    {
        if (null != Instance)
        {
            Debug.LogError($"InputManager 존재합니다!");
        }

        var loadPrefab = Resources.Load<GameManager>("Prefabs/Manager/GameManager");
        instance = Instantiate(loadPrefab);
        instance.transform.SetAsFirstSibling();
        IsExistInstance = true;
    }
    
    public GameObject mainPlayer;
    public Vector3 adjust_pos = new Vector3(0.0f, 0.1f, 4.0f);
    [SerializeField] private Canvas canvas;

    private List<NonPlayer> enemies = new List<NonPlayer>();
    private int enableCount = 0;
    private int totalCount = 0;
    private string clearText = "성공";
    
    private Dictionary<eState, Action> _actionMap = new();
    private Dictionary<eState, AttackInfoData> _attackInfoMap = new();
    private Dictionary<eRole, Role> _baseRoleMap = new();

    public void Init()
    {
        InputManager.CreateInstance();
        InputManager.Instance.Init();
        DataTable.LoadJsonData();
        LoadRoleState();
        LoadActions();
        LoadAttackInfo();
    }

    void Start()
    {
        // GetComponent<Camera>().fieldOfView = GetComponent<Camera>().fieldOfView;
        foreach(NonPlayer enemy in GameObject.FindObjectsOfType<NonPlayer>())
        {
            enemies.Add(enemy);
        }

        totalCount = enableCount = enemies.Count;
    }

    void Update()
    {
        if (Camera.main)
            Camera.main.transform.position = mainPlayer.transform.position + adjust_pos;
        
        if (InputManager.IsExistInstance)
            InputManager.Instance.Update();
    }

    public Transform GetCanvas() => canvas.transform;

    public void UpdateEnemyCount()
    {
        int remain = 0;
        foreach (NonPlayer enemy in enemies)
        {
            if (enemy.isActiveAndEnabled) remain++;
        }
        enableCount = remain;

        if (enableCount == 0)
        {
            OpenFinishDialog("성공");
        }
    }
    
    private void LoadRoleState()
    {
        var fightRole = new Role();
        fightRole.States.Add(eRoleState.IDLE, eState.FIGHTER_IDLE);
        fightRole.States.Add(eRoleState.WALK, eState.FIGHTER_WALK);
        fightRole.States.Add(eRoleState.RUN, eState.FIGHTER_RUN);
        fightRole.States.Add(eRoleState.RUN_STOP, eState.FIGHTER_RUN_STOP);
        fightRole.States.Add(eRoleState.JUMP_UP, eState.JUMP_UP);
        fightRole.States.Add(eRoleState.JUMP_DOWN, eState.JUMP_DOWN);
        fightRole.States.Add(eRoleState.LANDING, eState.LANDING);
        _baseRoleMap.Add(eRole.FIGHTER, fightRole);
        
        var rapierRole = new Role();
        rapierRole.States.Add(eRoleState.IDLE, eState.RAPIER_IDLE);
        rapierRole.States.Add(eRoleState.WALK, eState.RAPIER_WALK);
        rapierRole.States.Add(eRoleState.RUN, eState.RAPIER_RUN);
        rapierRole.States.Add(eRoleState.RUN_STOP, eState.RAPIER_RUN_STOP);
        rapierRole.States.Add(eRoleState.JUMP_UP, eState.RAPIER_JUMP_UP);
        rapierRole.States.Add(eRoleState.JUMP_DOWN, eState.RAPIER_JUMP_DOWN);
        rapierRole.States.Add(eRoleState.LANDING, eState.RAPIER_LANDING);
        _baseRoleMap.Add(eRole.RAPIER, rapierRole);
    }

    public Role GetRole(eRole role)
    {
        if (false == _baseRoleMap.ContainsKey(role))
            return null;
        return _baseRoleMap[role];
    }

    public void OpenFinishDialog(string text)
    {
        clearText = text;
        Transform clearObject = canvas.transform.Find("ClearDialog");
        clearObject.gameObject.SetActive(true);
    }

    public string GetEnableCount() => enableCount.ToString();

    public string GetTotalCount() => totalCount.ToString();

    public string GetClearText() => clearText;

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Application.Quit();
    }

    Vector3 playerPos = Vector3.zero;
    public void SetPlayerUIPos(Vector3 pos)
    {
        playerPos = pos;
    }

    public Vector3 GetPlayerUIPos() => playerPos;

    public void SetMainPlayer()
    {
        mainPlayer = FindObjectOfType<Player>().gameObject;
    }

    private void LoadActions()
    {
        var allActions = ActionTable.GetList();
        foreach (var actionData in allActions)
        {
            var state = UmUtil.StringToEnum<eState>(actionData.actionName);
            var action = new Action(actionData.actionName);
            action.Init();
            _actionMap.Add(state, action);
        }
    }
    
    private void LoadAttackInfo()
    {
        foreach (var action in _actionMap.Values)
        {
            var stateName = action.GetActionName();
            eState state = UmUtil.StringToEnum<eState>(stateName);
            if (_attackInfoMap.ContainsKey(state))
            {
                Debug.LogError($"Error _attackInfoMap contains key({state})");
                continue;
            }
            // action.CreateHitboxInfo()
            _attackInfoMap.Add(state, AttackInfoTable.GetData(stateName));
        }
    }
    
    public Action GetAction(eState curState)
    {
        if (false == _actionMap.ContainsKey(curState))
            return null;
        return _actionMap[curState];
    }
}
