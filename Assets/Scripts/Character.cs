using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Character : MonoBehaviour
{
    private float _walkSpeed = 0.005f;
    private float _runSpeed = 0.015f;
    private float _moveSpeed = 0.0f;

    protected Direction _direction;
    protected Dictionary<Direction, Vector3> _rotationMap = new Dictionary<Direction, Vector3>();
    protected Dictionary<Direction, Vector3> _moveMap = new Dictionary<Direction, Vector3>();
    protected Dictionary<Direction, KeyCode[]> _inputMap = new Dictionary<Direction, KeyCode[]>();
    protected List<Direction> _directionList = new List<Direction>();

    protected Dictionary<eState, State> stateMap = new Dictionary<eState, State>();

    private eState _prevState;
    public eState _curState;

    public bool _isGround = false;
    private bool _checkGround = true;

    public float _jumpPowerY = 6f;
    public float _jumpPowerXZ = 1f;

    public Vector3 _prevMoveSpeed = Vector3.zero;

    public float _hitDistance = 0.0f;
    public double _checkGroundDistance = 0.2;

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
                if (hit.distance <= _checkGroundDistance)
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
        _prevMoveSpeed = _moveMap[_direction] * _moveSpeed;
    }

    public void MoveDirectionPosition(Direction direction)
    {
        transform.position += _moveMap[direction] * _moveSpeed;
        _prevMoveSpeed = _moveMap[direction] * _moveSpeed;
    }

    public void ResetPrevMoveSpeed()
    {
        _prevMoveSpeed = Vector3.zero;
    }

    public void SetDirection(Direction direction)
    {
        _direction = direction;
        transform.eulerAngles = _rotationMap[_direction];
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

    public void ResetMoveSpeed()
    {
        _moveSpeed = 0.0f;
    }

    public void SetMoveSpeedToWalk()
    {
        _moveSpeed = _walkSpeed;
    }

    public void SetMoveSpeedToRun()
    {
        _moveSpeed = _runSpeed;
    }

    public void SetWalkSpeed(float walkSpeed)
    {
        _walkSpeed = walkSpeed;
    }

    public void SetRunSpeed(float runSpeed)
    {
        _runSpeed = runSpeed;
    }

    public float GetMoveSpeed()
    {
        return _moveSpeed;
    }

    public bool GetKeysDirection(Direction direction)
    {
        bool result = true;
        foreach (KeyCode keyCode in _inputMap[direction])
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

    public Vector3 GetJumpForce()
    {
        Vector3 result = _prevMoveSpeed * _jumpPowerXZ;
        result.y = _jumpPowerY;
        return result;
    }

    public eState GetPrevState()
    {
        return _prevState;
    }

    public bool IsGround()
    {
        return _isGround;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Vector3 vector3 = transform.position;
        vector3.y += 0.9f;
        Gizmos.DrawSphere(vector3, 5.0f);
    }

    public GameObject FindCollisions()
    {
        Vector3 vector3 = transform.position;
        vector3.y += 0.9f;
        Collider[] colliders = Physics.OverlapSphere(vector3, 5.0f);
        foreach (Collider collider in colliders)
        {
            Debug.Log("collider : " + collider.name);
            if(collider.name == "Player")
            {
                return collider.gameObject;
            }
        }

        return null;
    }

    public GameObject traceTarget = null;
    public void SetTarget(GameObject gameObject)
    {
        traceTarget = gameObject;
    }

    public GameObject GetTraceTarget()
    {
        return traceTarget;
    }
}