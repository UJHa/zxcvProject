using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

    public Camera camera;
    [FormerlySerializedAs("player")] public GameObject mainPlayer;
    public Vector3 adjust_pos = new Vector3(0.0f, 0.1f, 4.0f);
    [SerializeField]
    private Canvas canvas;

    private List<NonPlayer> enemies = new List<NonPlayer>();
    private int enableCount = 0;
    private int totalCount = 0;
    private string clearText = "성공";

    public void Init()
    {
        InputManager.CreateInstance();
        InputManager.Instance.Init();
    }

    void Start()
    {
        foreach(NonPlayer enemy in GameObject.FindObjectsOfType<NonPlayer>())
        {
            enemies.Add(enemy);
        }

        totalCount = enableCount = enemies.Count;
    }

    void Update()
    {
        if (camera)
            camera.transform.position = mainPlayer.transform.position + adjust_pos;
        
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
}
