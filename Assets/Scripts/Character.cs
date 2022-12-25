using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StateInfo
{
    public eState state;
    public eStateType stateType;
}

public enum eStateType
{
    INPUT,
    NONE,
}

// 엄todo : 정밀한 바닥 처리(1차 작업 완료)
// 엄todo : 공격 기능 개발
// 엄todo : 언덕 오르기
public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _fullHp = 100f;
    [SerializeField] private float _attackPower = 30f;
    
    [Header("[ Anim CrossFadeTime ]")]
    [SerializeField] public float jumpUpStart = 0.07f;
    [SerializeField] public float idleStart = 0.07f;
    [SerializeField] public float walkStart = 0.07f;
    [SerializeField] public float runStart = 0.07f;
    [SerializeField] public float jumpEnd = 0.07f;
    
    
    [SerializeField] private AnimationCurve _jumpUp = new ();
    [SerializeField] private AnimationCurve _jumpDown = new ();
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _moveSpeed = 0.0f;
    [Header("Jump Stats")]
    [SerializeField] private bool _useReverse = false;
    [SerializeField] private float _jumpMaxHeight = 2f;
    [SerializeField] private float _jumpUpMaxTimer = 2f;
    [SerializeField] private float _jumpDownMaxTimer = 1f;
    [SerializeField] private float _jumpPowerY = 6f;
    [SerializeField] private float _jumpPowerXZ = 1f;
    [SerializeField] private float _idleJumpSpeedRate = 0.1f;
    [SerializeField] private float _walkJumpSpeedRate = 0.3f;
    [SerializeField] private float _runJumpSpeedRate = 1f;

    [Header("JumpStats 2")]
    [SerializeField] public float _jumpOffset = 0.31f;
    [SerializeField] public Vector3 _groundCheckBoxSize = new Vector3(1f, 0.2f, 1f);
    
    [SerializeField] private float _interpolationHeight = 0.25f;

    [Header("3D Phygics Component")] 
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ForceMode _forceModeType = ForceMode.Force;

    [Header("UI")]
    protected Slider slider;

    private float _curHp = 0f;
    protected float sliderScale = 1.0f;

    protected Vector3 _directionVector = Vector3.zero;

    protected Dictionary<eState, State> stateMap = new();

    public eState _prevState;
    public eState _curState;

    public bool _isGround = false;
    private bool _checkGround = true;

    private List<StateInfo> _changeStates = new();

    private Tween _rotationTween = null;

    public double CHECK_GROUND_DISTANCE = 0.2;

    private void Awake()
    {
        MakeFixedDeltaTimeCurve(_jumpUp, _jumpUpMaxTimer);
        if (false == _useReverse)
            MakeFixedDeltaTimeCurve(_jumpDown, _jumpDownMaxTimer);
        else
        {
            MakeReverseCurve(_jumpUp, _jumpDown);
        }
    }

    private void MakeFixedDeltaTimeCurve(AnimationCurve curve, float argMaxTime)
    {
        // 초단위로 커브 만들고 maxTime 통해서 커브 시간 변경
        List<float> temp = new();
        var divideLength = Time.fixedDeltaTime * 1f;
        var count = (int)(1f / divideLength);
        for (int i = 0; i < count; i++)
        {
            temp.Add(curve.Evaluate(divideLength * i));
        }
        temp.Add(1f);
        
        while (curve.keys.Length > 0)
        {
            curve.RemoveKey(0);
        }

        for (int i = 0; i < temp.Count; i++)
        {
            curve.AddKey(i * divideLength * argMaxTime, temp[i]);
            Debug.Log($"[testum]key({i * Time.fixedDeltaTime * argMaxTime})/val({temp[i]})");
        }
    }

    private void MakeReverseCurve(AnimationCurve origin, AnimationCurve reverse)
    {
        var maxX = origin[origin.length - 1].time;
        var maxY = origin[origin.length - 1].value;
        for (int i = 0; i < origin.length; i++)
        {
            Debug.Log($"i({i}) time({origin[i].time}) value({origin[i].value})");
            reverse.AddKey(maxX-origin[i].time, maxY-origin[i].value);
        }
    }

    public float GetJumpUpVelocity(float deltatime)
    {
        if (deltatime <= 0f)
            return 0f;
        var prevTime = deltatime - Time.fixedDeltaTime;
        float dx = deltatime - prevTime;
        float dy = _jumpUp.Evaluate(deltatime) - _jumpUp.Evaluate(prevTime);
        return dy / dx * _jumpMaxHeight;
    }
    
    public float GetJumpDownVelocity(float deltatime)
    {
        if (deltatime <= 0f)
            return 0f;
        
        var dx = Time.fixedDeltaTime;
        var curTime = deltatime;
        if (deltatime >= _jumpDownMaxTimer)
            curTime = _jumpDownMaxTimer;
        var dy = _jumpDown.Evaluate(curTime) - _jumpDown.Evaluate(curTime - dx);
        
        return -1f * dy / dx * _jumpMaxHeight;
        
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
        //Debug.Log($"tranform height : {transform.position.y}");
        stateMap[_curState].FixedUpdateState();
    }

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;
        center.y -= _groundCheckBoxSize.y / 2;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, _groundCheckBoxSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (_changeStates.Count > 0)
        {
            // 엄todo : 이 시점에 _curState를 queue에 저장하면 되겠지? queue data클래스는 {이전 프레임 시간, List<StateInfo>} 이렇게 구성하면 될듯?
            Debug.Log($"[testState]Change prev({_curState}) count({_changeStates.Count})");
            if (_changeStates.Count == 1)
            {
                eState state = _changeStates[0].state;
                stateMap[_curState].EndState();
                stateMap[state].StartState();
                _prevState = _curState;
                _curState = state;
            }
            else
            {
                eState state = GetNextState();
                stateMap[_curState].EndState();
                stateMap[state].StartState();
                _prevState = _curState;
                _curState = state;
            }
            _changeStates.Clear();
        }
        stateMap[_curState].UpdateState();

        UpdateUI();
    }

    private eState GetNextState()
    {
        foreach (var stateInfo in _changeStates)
        {
            if (stateInfo.stateType == eStateType.INPUT)
                return stateInfo.state;
        }

        return _changeStates[0].state;
    }

    public eState GetPrevState()
    {
        return _prevState;
    }

    public void FixedUpdatePhygics()
    {
        if (_checkGround)
        {
            int layerMask = 1;
            layerMask = layerMask << LayerMask.NameToLayer("Ground");
            RaycastHit[] hits = Physics.BoxCastAll(GetGroundBoxCenter(), GetGroundBoxHalfSize(), Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
            
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
    }
    
    public void MovePosition(Vector3 direction)
    {
        // transform.position += _moveMap[direction] * _moveSpeed;
        _rigidbody.velocity = direction * _moveSpeed;
    }
    
    public void SetDirectionByVector3(Vector3 argVector)
    {
        _directionVector = argVector;
        var euler = GetEuler(_directionVector);
        if (null != _rotationTween)
            _rotationTween.Kill();
        var rot = GetRotation(_directionVector);
        _rotationTween = transform.DORotateQuaternion(rot, 0.1f);
        // transform.eulerAngles = euler;
    }

    public Vector3 GetDirectionVector()
    {
        return _directionVector;
    }
    
    public float GetJumpUpMaxTimer()
    {
        return _jumpUpMaxTimer;
    }
    
    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }
    
    private Quaternion GetRotation(Vector3 argVector)
    {
        var rot = Quaternion.LookRotation(argVector);
        return rot;
    }
    
    private Vector3 GetEuler(Vector3 argVector)
    {
        var rot = Quaternion.LookRotation(argVector);
        return rot.eulerAngles;
    }

    public void ChangeState(eState state, eStateType stateType = eStateType.NONE)
    {
        Debug.Log($"[testState]State Change prev({_curState}) cur({state})");
        // _curState = state;
        _changeStates.Add(new StateInfo()
        {
            state = state,
            stateType = stateType
        });
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

    public bool IsGroundCheck()
    {
        int layerMask = 1;
        layerMask = layerMask << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetGroundBoxCenter(), GetGroundBoxHalfSize(), Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        return hits.Length > 0;
    }

    public RaycastHit[] GetGroundCheckObjects()
    {
        int layerMask = 1;
        layerMask = layerMask << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetGroundBoxCenter(), GetGroundBoxHalfSize(), Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        return hits;
    }

    public void SetPositionY(float groundHeight)
    {
        Debug.Log($"groundHeight({groundHeight})");
        var transform1 = transform;
        Vector3 pos = transform1.position;
        transform1.position = new Vector3(pos.x, groundHeight, pos.z);
    }

    public void UpdateGroundHeight()
    {
        float groundHeight = float.MinValue;
        var rayObjs = GetGroundCheckObjects();
        foreach (var rayObj in rayObjs)
        {
            if (rayObj.transform.TryGetComponent<Ground>(out var ground))
            {
                if (groundHeight < ground.heightPosY)
                {
                    groundHeight = ground.heightPosY;
                }
            }
        }
        if (false == groundHeight.Equals(float.MinValue) && Math.Abs(groundHeight - transform.position.y) < _interpolationHeight)
            SetPositionY(groundHeight);
    }

    private Vector3 GetGroundBoxHalfSize()
    {
        return _groundCheckBoxSize / 2;
    }
    
    private Vector3 GetGroundBoxCenter()
    {
        Vector3 boxCenter = transform.position;
        boxCenter.y -= _groundCheckBoxSize.y / 2;
        return boxCenter;
    }
}