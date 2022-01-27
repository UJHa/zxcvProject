using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Character : MonoBehaviour
{
    protected Slider slider;
    protected float sliderScale = 1.0f;

    private float _fullHp = 100f;
    private float _curHp = 0f;

    private float _attackPower = 30f;

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
    public double CHECK_GROUND_DISTANCE = 0.2;

    protected virtual void StartUI()
    {
        _curHp = _fullHp;
        GameObject prefab = Resources.Load("Prefabs/HpSlider") as GameObject;
        GameObject hpSlider = GameObject.Instantiate(prefab);
        
        hpSlider.transform.SetParent(GameManager.Instance.GetCanvas());
        hpSlider.transform.SetAsFirstSibling();
        //hpSlider.transform.localScale = Vector3.one;
        slider = hpSlider.GetComponent<Slider>();

        slider.gameObject.SetActive(true);
    }

    protected virtual void UpdateUI()
    {
        Vector3 sliderPos = transform.position;
        sliderPos.y += 2f;
        slider.transform.position = Camera.main.WorldToScreenPoint(sliderPos);

        Vector3 playerUIDistance = Camera.main.transform.position - GameManager.Instance.GetPlayerUIPos();
        Vector3 currentUIDistance = Camera.main.transform.position - sliderPos;

        sliderScale = Vector3.Magnitude(playerUIDistance) / Vector3.Magnitude(currentUIDistance);
        slider.gameObject.transform.localScale = Vector3.one * sliderScale;
        slider.value = _curHp / _fullHp;
    }

    private void FixedUpdate()
    {
        if (_checkGround)
        {
            Vector3 curPos = transform.position;
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 0;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;


            RaycastHit hit;
            if (Physics.Raycast(curPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.red);
                _isGround = false;
                if (hit.distance <= CHECK_GROUND_DISTANCE)
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

        UpdateUI();
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
        //Gizmos.color = Color.yellow;
        //Vector3 vector3 = transform.position;
        //vector3.y += 0.9f;
        //Gizmos.DrawSphere(vector3, findRange);
    }

    public GameObject FindCollisions()
    {
        Vector3 vector3 = transform.position;
        vector3.y += 0.9f;
        Collider[] colliders = Physics.OverlapSphere(vector3, findRange);
        foreach (Collider collider in colliders)
        {
            if(collider.name == "Player")
            {
                return collider.gameObject;
            }
        }

        return null;
    }

    public GameObject traceTarget = null;
    private float findRange = 5.0f;
    public void SetTarget(GameObject gameObject)
    {
        traceTarget = gameObject;
    }

    public GameObject GetTraceTarget()
    {
        return traceTarget;
    }

    public bool IsInRange()
    {
        return (Vector3.Distance(traceTarget.transform.position, transform.position) > findRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 피해 받았을때 진입
        // other : attacker
        // name : defender
        if (other.name != "AttackCollider")
            return;

        Character attacker = other.transform.parent.GetComponent<Character>();
        if (attacker == null)
        {
            Debug.Log("Attacker is not character.");
            return;
        }
        
        _curHp -= attacker.getAttackDamage();
        if (_curHp <= 0f)
        {
            _curHp = 0f;
            ChangeState(eState.DEAD);
        }
    }

    public float getAttackDamage()
    {
        return _attackPower;
    }

    public virtual void DeadDisable()
    {
        gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
    }
}