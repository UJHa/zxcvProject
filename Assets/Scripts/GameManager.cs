using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera camera;
    public GameObject player;
    public Vector3 adjust_pos = new Vector3(0.0f, 0.1f, 4.0f);

    void Start()
    {
        
    }

    void Update()
    {
        camera.transform.position = player.transform.position + adjust_pos;
    }
}
