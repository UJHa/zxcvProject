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
    private eState curState;


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

        stateMap.Add(eState.IDLE, new IdleState(this));
        stateMap.Add(eState.RUN, new RunState(this));

        curState = eState.IDLE;

        prevPositionY = transform.position.y;
    }

    private void FixedUpdate()
    {
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

    public void SetNextState(eState state)
    {
        curState = state;
    }

    public string GetRunParameter()
    {
        return key_isRun;
    }
}
