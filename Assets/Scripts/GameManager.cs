using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera camera;
    public Player player;
    public Vector3 adjust_pos = new Vector3(0.0f, 2.0f, 5.0f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 한글 주석 테스트
        camera.transform.position = player.transform.position + adjust_pos;
    }
}
