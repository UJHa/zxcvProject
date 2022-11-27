using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    private float _fullHp = 100f;
    private float _attackPower = 30f;
    
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _moveSpeed = 0.0f;

    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _jumpPowerY = 6f;
    [SerializeField] private float _jumpPowerXZ = 1f;

    [Header("JumpStats")]
    public float _jumpOffset = 0.31f;
    public float _downBoxHeight = 0.2f;

    [Header("3D Phygics Component")] 
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ForceMode _forceModeType = ForceMode.Force;

    [Header("UI")]
    protected Slider slider;

    private float _curHp = 0f;
    protected float sliderScale = 1.0f;

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

    public float _hitDistance = 0.0f;
    public double CHECK_GROUND_DISTANCE = 0.2;

    private void Awake()
    {
        
    }

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
        // Debug.Log($"IsGround : {_isGround}");
        if (_checkGround)
        {
            Vector3 boxCenter = transform.position;
            boxCenter.y -= _downBoxHeight / 2;
            Vector3 boxHalfSize = new Vector3(1f, _downBoxHeight, 1f) / 2;  // 캐스트할 박스 크기의 절반 크기. 이렇게 하면 가로 2 세로 2 높이 2의 크기의 공간을 캐스트한다.
            int layerMask = 1;
            layerMask = layerMask << LayerMask.NameToLayer("Ground");
            RaycastHit[] hits = Physics.BoxCastAll(boxCenter, boxHalfSize, Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
            
            //if (hits.Length > 1)
            //{
            //    Debug.Log("=====");
            //    foreach (var hi in hits)
            //    {
            //        if (_downBoxHeight > Vector3.Distance(boxCenter, hi.transform.position))
            //            Debug.Log($"name : {hi.collider.name} distance : {Vector3.Distance(boxCenter, hi.transform.position)}");
            //    }
            //}
            if (hits.Length > 0)
            {
                //Debug.Log("Ground Box!!!");
                _isGround = true;
            }

            // todo
            // 바닥으로 레이져 쏴서 모든 ground의 접점 거리 체크하기
            // 접점이 가장 짧은 길이가 height보다 작으면 바닥으로 취급시키기
            // >> 점프 시작 상태일 때 높이가 up 벡터 방향으로 이동 시 바닥을 무시하도록 변경
            // 
            // 점프 시작 시 바닥으로 변경하는 처리가 필요 
            // // + 기존에 0.2초 딜레이 후 바닥체크를 시작했었음...(JumpState 참고)
            // // + 시간 초 제거 후 JumpState 상태 시 y값이 최대값이 아니게 될 때부터 바닥 체크
            // // + fall 코드 한번만 더 생각해보자

            //RaycastHit hit;
            //if (Physics.Raycast(curPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            //{
            //    Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.red);
            //    _isGround = false;
            //    if (hit.distance <= CHECK_GROUND_DISTANCE)
            //    {
            //        _isGround = true;
            //        Debug.Log("Ground Check!!!");
            //    }
            //}
            //else
            //{
            //    Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            //    _isGround = true;
            //    Debug.Log("Ground not ray!!!");
            //}
        }
        //Debug.Log($"tranform height : {transform.position.y}");
    }

    private void OnDrawGizmos()
    {
        float height = 0.2f;
        Vector3 center = transform.position;
        center.y -= height / 2;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, new Vector3(1f, 0.2f, 1f));
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
        // transform.position += _moveMap[direction] * _moveSpeed;
        _rigidbody.velocity = _moveMap[direction] * _moveSpeed;
    }

    public void ResetPrevMoveSpeed()
    {
        // _prevMoveSpeed = Vector3.zero;
    }

    public void SetDirection(Direction direction)
    {
        _direction = direction;
        transform.eulerAngles = _rotationMap[_direction];
    }
    
    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
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
        // 점프한 y 좌표는 따로 관리해야 될듯?
        _rigidbody.velocity = Vector3.zero;
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
        Vector3 result = _rigidbody.velocity * _jumpPowerXZ;
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

    public ForceMode GetForceModeType()
    {
        return _forceModeType;
    }
}