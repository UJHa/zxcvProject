using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Direction _direction;
    private float _walkSpeed = 0.005f;
    private float _runSpeed = 0.015f;
    private float _moveSpeed = 0.0f;
    private float _prevPositionY;

    private Dictionary<Direction, Vector3> _rotationMap = new Dictionary<Direction, Vector3>();
    private Dictionary<Direction, Vector3> _moveMap = new Dictionary<Direction, Vector3>();
    private Dictionary<Direction, KeyCode[]> _inputMap = new Dictionary<Direction, KeyCode[]>();
    private List<Direction> _directionList = new List<Direction>();

    private Dictionary<eState, State> stateMap = new Dictionary<eState, State>();

    private eState _prevState;
    public eState _curState;

    public bool _isGround = false;
    private bool _checkGround = true;

    // Start is called before the first frame update
    void Start()
    {
        _rotationMap.Add(Direction.FRONT, new Vector3(0, 0, 0));
        _rotationMap.Add(Direction.BACK, new Vector3(0, 180, 0));
        _rotationMap.Add(Direction.LEFT, new Vector3(0, 90, 0));
        _rotationMap.Add(Direction.RIGHT, new Vector3(0, -90, 0));
        _rotationMap.Add(Direction.LEFT_FRONT, new Vector3(0, 45, 0));
        _rotationMap.Add(Direction.RIGHT_FRONT, new Vector3(0, -45, 0));
        _rotationMap.Add(Direction.LEFT_BACK, new Vector3(0, 135, 0));
        _rotationMap.Add(Direction.RIGHT_BACK, new Vector3(0, -135, 0));

        float diagonal = Mathf.Sqrt(2f) / 2f;

        _moveMap.Add(Direction.FRONT, new Vector3(0, 0, 1));
        _moveMap.Add(Direction.BACK, new Vector3(0, 0, -1));
        _moveMap.Add(Direction.LEFT, new Vector3(1, 0, 0));
        _moveMap.Add(Direction.RIGHT, new Vector3(-1, 0, 0));
        _moveMap.Add(Direction.LEFT_FRONT, new Vector3(diagonal, 0, diagonal));
        _moveMap.Add(Direction.RIGHT_FRONT, new Vector3(-diagonal, 0, diagonal));
        _moveMap.Add(Direction.LEFT_BACK, new Vector3(diagonal, 0, -diagonal));
        _moveMap.Add(Direction.RIGHT_BACK, new Vector3(-diagonal, 0, -diagonal));

        _inputMap.Add(Direction.FRONT, new KeyCode[] { KeyCode.DownArrow });
        _inputMap.Add(Direction.BACK, new KeyCode[] { KeyCode.UpArrow });
        _inputMap.Add(Direction.LEFT, new KeyCode[] { KeyCode.LeftArrow });
        _inputMap.Add(Direction.RIGHT, new KeyCode[] { KeyCode.RightArrow });
        _inputMap.Add(Direction.LEFT_FRONT, new KeyCode[] { KeyCode.DownArrow, KeyCode.LeftArrow });
        _inputMap.Add(Direction.RIGHT_FRONT, new KeyCode[] { KeyCode.DownArrow, KeyCode.RightArrow });
        _inputMap.Add(Direction.LEFT_BACK, new KeyCode[] { KeyCode.UpArrow, KeyCode.LeftArrow });
        _inputMap.Add(Direction.RIGHT_BACK, new KeyCode[] { KeyCode.UpArrow, KeyCode.RightArrow });

        _directionList.Add(Direction.FRONT);
        _directionList.Add(Direction.BACK);
        _directionList.Add(Direction.LEFT);
        _directionList.Add(Direction.RIGHT);
        _directionList.Add(Direction.LEFT_FRONT);
        _directionList.Add(Direction.RIGHT_FRONT);
        _directionList.Add(Direction.LEFT_BACK);
        _directionList.Add(Direction.RIGHT_BACK);

        //transform.Rotate(0, 90, 0);
        _direction = Direction.FRONT;

        stateMap.Add(eState.IDLE, new IdleState(this));
        stateMap.Add(eState.WALK, new WalkState(this));
        stateMap.Add(eState.RUN, new RunState(this));
        stateMap.Add(eState.JUMP, new JumpState(this));

        _curState = eState.IDLE;

        stateMap[_curState].StartState();

        _prevPositionY = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (_checkGround)
        {
            Vector3 curPos = transform.position;
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;


            RaycastHit hit;
            if (Physics.Raycast(curPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                // test for jump
                Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.red);
                _isGround = false;
                // todo : change 3.3e-06 to const value
                if (hit.distance <= 3.3E-06)
                {
                    _isGround = true;
                }
            }
            else
            {
                Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.white);
                _isGround = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _prevState = _curState;
        stateMap[_curState].UpdateState();
        if (_prevState != _curState)
        {
            stateMap[_prevState].EndState();
            stateMap[_curState].StartState();
        }
    }

    public void MovePosition()
    {
        transform.position += _moveMap[_direction] * _moveSpeed;
        transform.eulerAngles = _rotationMap[_direction];
    }

    public void SetDirection(Direction direction)
    {
        this._direction = direction;
    }

    public Direction GetDirection()
    {
        return _direction;
    }

    public void ChangeState(eState state)
    {
        _curState = state;
    }

    public List<Direction> GetDirections()
    {
        return _directionList;
    }

    public void SetWalkSpeed()
    {
        _moveSpeed = _walkSpeed;
    }

    public void SetRunSpeed()
    {
        _moveSpeed = _runSpeed;
    }

    public bool GetKeysDirection(Direction direction)
    {
        bool result = true;
        foreach(KeyCode keyCode in _inputMap[direction])
        {
            result = Input.GetKey(keyCode) && result;
        }
        return result;
    }

    public bool GetKeysDownDirection(Direction direction)
    {
        bool result = true;
        foreach (KeyCode keyCode in _inputMap[direction])
        {
            result = Input.GetKeyDown(keyCode) && result;
        }
        return result;
    }

    public bool GetKeysUpDirection(Direction direction)
    {
        bool result = true;
        foreach (KeyCode keyCode in _inputMap[direction])
        {
            result = Input.GetKeyUp(keyCode) && result;
        }
        return result;
    }

    public void StartJump()
    {
        _isGround = false;
        _checkGround = false;
    }

    public void CheckIsGround()
    {
        _checkGround = true;
    }

    public bool IsGround()
    {
        return _isGround;
    }
}
