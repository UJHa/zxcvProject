using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Direction direction;
    private float moveSpeed = 0.03f;
    private float prevPositionY;

    private Dictionary<Direction, Vector3> rotationMap = new Dictionary<Direction, Vector3>();
    private Dictionary<Direction, Vector3> moveMap = new Dictionary<Direction, Vector3>();

    private Dictionary<eState, State> stateMap = new Dictionary<eState, State>();

    private eState prevState;
    public eState curState;

    public bool isGround = false;

    private float footPosY;

    // Start is called before the first frame update
    void Start()
    {
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

        stateMap.Add(eState.IDLE, new IdleState(this));
        stateMap.Add(eState.RUN, new RunState(this));
        stateMap.Add(eState.JUMP, new JumpState(this));

        curState = eState.IDLE;

        prevPositionY = transform.position.y;

        //footPosY = transform.position.y - GetComponent<CapsuleCollider>().height / 2.0f;
    }

    private void FixedUpdate()
    {
        Vector3 curPos = transform.position;
        curPos.y = transform.position.y - GetComponent<CapsuleCollider>().height / 2.0f; ;
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;


        RaycastHit hit;
        if (Physics.Raycast(curPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.yellow);
            Debug.Log("Did Hit");
            Debug.Log(hit.distance);
            isGround = false;
            // todo : 바닥에서 밀어낸 반발력으로 인한 공중에 뜬 공백의 최솟값을 상수로 관리할 필요 있음
            if (hit.distance <= 3.3E-06)
                isGround = true;
        }
        else
        {
            Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            Debug.Log("Did not Hit");
            isGround = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        prevState = curState;
        stateMap[curState].UpdateState();
        if(prevState != curState)
        {
            stateMap[prevState].EndState();
            stateMap[curState].StartState();
        }
    }

    public void MovePosition()
    {
        transform.position += moveMap[direction] * moveSpeed;
        transform.eulerAngles = rotationMap[direction];
    }

    public void SetDirection(Direction direction)
    {
        this.direction = direction;
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public void ChangeState(eState state)
    {
        curState = state;
    }

    public void StartJump()
    {
        isGround = false;
    }

    public bool IsGround()
    {
        return isGround;
    }
}
