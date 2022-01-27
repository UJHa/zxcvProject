using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CGGameManager : MonoBehaviour
{
    private static CGGameManager instance = null;
    public static CGGameManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
        }

        instance = this;
    }

    public Camera camera;
    public GameObject player;

    private Dictionary<string, KeyCode> _skillKeyMap = new Dictionary<string, KeyCode>();

    // Change value in Unity editor
    [SerializeField]
    private Vector3 adjust_pos = new Vector3(0.0f, 12f, 8.0f);

    void Start()
    {
        _skillKeyMap.Add("Skill 1", KeyCode.Mouse0); // left mouse
        _skillKeyMap.Add("Skill 2", KeyCode.Mouse1); // right mouse
        _skillKeyMap.Add("Skill 3", KeyCode.Q);      // wheel up
        _skillKeyMap.Add("Skill 4", KeyCode.E);      // wheel down
        _skillKeyMap.Add("Skill 5", KeyCode.E);      // 
    }

    void Update()
    {
        if (camera)
        {
            Vector3 cameraCenterPos = player.transform.position;
            cameraCenterPos.y = 0f;
            camera.transform.position = cameraCenterPos + adjust_pos;
        }

        foreach (KeyValuePair<string, KeyCode> keymap in _skillKeyMap)
        {
            if (Input.GetKeyDown(keymap.Value))
            {
                Debug.Log("Key Down : " + keymap.Key);
            }
        }
    }
}
