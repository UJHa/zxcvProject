using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Direction direction;
    public float moveSpeed = 0.03f;
    private Dictionary<Direction, Vector3> rotationMap = new Dictionary<Direction, Vector3>();
    private Dictionary<Direction, Vector3> moveMap = new Dictionary<Direction, Vector3>();

    private Animator animator;

    private const string key_isRun = "IsRun";
    private const string key_isAttack01 = "IsAttack01";
    private const string key_isAttack02 = "IsAttack02";
    private const string key_isJump = "IsJump";
    private const string key_isDamage = "IsDamage";
    private const string key_isDead = "IsDead";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        rotationMap.Add(Direction.FRONT, new Vector3(0, 0, 0));
        rotationMap.Add(Direction.BACK, new Vector3(0, 180, 0));
        rotationMap.Add(Direction.LEFT, new Vector3(0, 90, 0));
        rotationMap.Add(Direction.RIGHT, new Vector3(0, -90, 0));
        rotationMap.Add(Direction.LEFT_FRONT, new Vector3(0, 45, 0));
        rotationMap.Add(Direction.RIGHT_FRONT, new Vector3(0, -45, 0));
        rotationMap.Add(Direction.LEFT_BACK, new Vector3(0, 135, 0));
        rotationMap.Add(Direction.RIGHT_BACK, new Vector3(0, -135, 0));

        moveMap.Add(Direction.FRONT, new Vector3(0, 0, 1));
        moveMap.Add(Direction.BACK, new Vector3(0, 0, -1));
        moveMap.Add(Direction.LEFT, new Vector3(1, 0, 0));
        moveMap.Add(Direction.RIGHT, new Vector3(-1, 0, 0));
        moveMap.Add(Direction.LEFT_FRONT, new Vector3(1, 0, 1));
        moveMap.Add(Direction.RIGHT_FRONT, new Vector3(-1, 0, 1));
        moveMap.Add(Direction.LEFT_BACK, new Vector3(1, 0, -1));
        moveMap.Add(Direction.RIGHT_BACK, new Vector3(-1, 0, -1));

        //transform.Rotate(0, 90, 0);
        direction = Direction.FRONT;
    }

    // Update is called once per frame
    void Update()
    {
        // 동시 입력에 대한 처리부터.. 대각 방향에 대한 입력 처리를 우선한다.
        // 이동
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Direction.LEFT_FRONT;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            direction = Direction.RIGHT_FRONT;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Direction.LEFT_BACK;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            direction = Direction.RIGHT_BACK;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            direction = Direction.FRONT;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = Direction.BACK;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Direction.LEFT;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = Direction.RIGHT;
        }

        if (Input.GetKey(KeyCode.UpArrow) || (Input.GetKey(KeyCode.DownArrow)) || (Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.RightArrow)))
        {
            animator.SetBool(key_isRun, true);
            transform.position += moveMap[direction] * moveSpeed;
            transform.eulerAngles = rotationMap[direction];
        }
        else
        {
            animator.SetBool(key_isRun, false);
        }
    }
}
