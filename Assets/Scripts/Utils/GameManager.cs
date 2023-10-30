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
    
    private Dictionary<ActionKey, Action> _actionMap = new();
    private Dictionary<ActionKey, AttackInfoData> _attackInfoMap = new();
    private Dictionary<eRole, Role> _baseRoleMap = new();
    private AnimationCurveCollection _animationCurveCollection;

    public void Init()
    {
        InputManager.CreateInstance();
        InputManager.Instance.Init();
        DataTable.LoadJsonData();
        LoadRoleState();
        LoadActions();
        LoadAttackInfo();
        LoadAnimCurve();
    }

    void Start()
    {
    }

    void Update()
    {
        if (Camera.main)
            Camera.main.transform.position = mainPlayer.transform.position + adjust_pos;
        
        if (InputManager.IsExistInstance)
            InputManager.Instance.Update();
    }

    public Transform GetCanvas() => canvas.transform;
    
    private void LoadRoleState()
    {
        // 엄todo : role 관리 json으로 분리하기
        var fightRole = new Role();
        fightRole.States.Add(eRoleState.IDLE, ActionKey.FIGHTER_IDLE);
        fightRole.States.Add(eRoleState.WALK, ActionKey.FIGHTER_WALK);
        fightRole.States.Add(eRoleState.RUN, ActionKey.FIGHTER_RUN);
        fightRole.States.Add(eRoleState.RUN_STOP, ActionKey.FIGHTER_RUN_STOP);
        fightRole.States.Add(eRoleState.JUMP_UP, ActionKey.JUMP_UP);
        fightRole.States.Add(eRoleState.JUMP_DOWN, ActionKey.JUMP_DOWN);
        fightRole.States.Add(eRoleState.LANDING, ActionKey.LANDING);
        _baseRoleMap.Add(eRole.FIGHTER, fightRole);
        
        var rapierRole = new Role();
        rapierRole.States.Add(eRoleState.IDLE, ActionKey.RAPIER_IDLE);
        rapierRole.States.Add(eRoleState.WALK, ActionKey.RAPIER_WALK);
        rapierRole.States.Add(eRoleState.RUN, ActionKey.RAPIER_RUN);
        rapierRole.States.Add(eRoleState.RUN_STOP, ActionKey.RAPIER_RUN_STOP);
        rapierRole.States.Add(eRoleState.JUMP_UP, ActionKey.RAPIER_JUMP_UP);
        rapierRole.States.Add(eRoleState.JUMP_DOWN, ActionKey.RAPIER_JUMP_DOWN);
        rapierRole.States.Add(eRoleState.LANDING, ActionKey.RAPIER_LANDING);
        _baseRoleMap.Add(eRole.RAPIER, rapierRole);
    }

    public Role GetRole(eRole role)
    {
        if (false == _baseRoleMap.ContainsKey(role))
            return null;
        return _baseRoleMap[role];
    }

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
            var state = UmUtil.StringToEnum<ActionKey>(actionData.actionName);
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
            ActionKey state = UmUtil.StringToEnum<ActionKey>(stateName);
            if (_attackInfoMap.ContainsKey(state))
            {
                Debug.LogError($"Error _attackInfoMap contains key({state})");
                continue;
            }
            // action.CreateHitboxInfo()
            _attackInfoMap.Add(state, AttackInfoTable.GetData(stateName));
        }
    }

    private void LoadAnimCurve()
    {
        var animCurveCollection = Resources.Load<AnimationCurveCollection>("Prefabs/Utils/AnimationCurveCollection");
        _animationCurveCollection = Instantiate(animCurveCollection);
    }

    public AnimationCurve CreateAnimCurve(string key)
    {
        return _animationCurveCollection.CreateAnimCurve(key);
    }
    
    public AnimationCurve CreateAnimCurve(string key, float xMaxValue)
    {
        return _animationCurveCollection.CreateAnimCurve(key, xMaxValue);
    }
    
    public float GetCurveVelocity(AnimationCurve curve, float curDeltatime, float xMaxValue, float yMaxValue)
    {
        var curTime = Mathf.Clamp(curDeltatime, 0f, xMaxValue);
        if (curDeltatime <= 0f)
            return 0f;
        
        var xRate = 1f / xMaxValue;
        var prevTime = curTime - Time.fixedDeltaTime;
        
        var dx = Time.fixedDeltaTime;
        var dy = curve.Evaluate(curTime * xRate) - curve.Evaluate(prevTime * xRate);
        return dy / dx * yMaxValue;
    }
    
    public AnimationCurve GetAnimCurve(string key)
    {
        return _animationCurveCollection.GetAnimCurve(key);
    }
    
    public Action GetAction(ActionKey curState)
    {
        if (false == _actionMap.ContainsKey(curState))
            return null;
        return _actionMap[curState];
    }
}
