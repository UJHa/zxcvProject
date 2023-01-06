using System;
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

public class ColliderInfo
{
    public GameObject cObject;
    public eWallDirection colliderType;
}

public enum eWallDirection
{
    LEFT,
    RIGHT,
    FRONT,
    BACK
}

// 엄todo : 정밀한 바닥 처리(1차 작업 완료)
// 엄todo : 벽 비비기 중력 처리(완료)
// 엄todo : 벽 비비기 대각 이동 처리(완료)
// 엄todo : 벽 비비기 대각벽에서의 대각 이동 버그 처리
// 엄todo : 서버가 붙으면 어떻게 위치에 대한 보간을 처리할지
// 엄todo : 공격 기능 개발
// 엄todo : 언덕 오르기
public class Character : MonoBehaviour
{
    [SerializeField] private float _testPosX = -3.5f;
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
    [SerializeField] public float _wallBoxThickness = 0.05f;
    [SerializeField] public float _wallBoxWidth = 0.2f;
    [SerializeField] public float _wallBoxHeight = 1f;
    
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

    private RaycastHit[] _groundObjs = null;
    private Dictionary<eWallDirection, List<ColliderInfo>> _colliderInfos = new();
    private RaycastHit[] _leftWallObjs = null;
    private RaycastHit[] _rightWallObjs = null;
    private RaycastHit[] _frontWallObjs = null;
    private RaycastHit[] _backWallObjs = null;

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
        // ground check
        _groundObjs = GetGroundCheckObjects();
        
        _colliderInfos.Clear();
        _colliderInfos.Add(eWallDirection.LEFT, new());
        _colliderInfos.Add(eWallDirection.RIGHT, new());
        _colliderInfos.Add(eWallDirection.FRONT, new());
        _colliderInfos.Add(eWallDirection.BACK, new());
        {
            RaycastHit[] hits = GetWallCheckObjects(Vector3.left);
            foreach (var hit in hits)
            {
                _colliderInfos[eWallDirection.LEFT].Add(new()
                {
                    cObject = hit.collider.gameObject,
                    colliderType = eWallDirection.LEFT
                });
            }
        }
        {
            RaycastHit[] hits = GetWallCheckObjects(Vector3.right);
            foreach (var hit in hits)
            {
                _colliderInfos[eWallDirection.RIGHT].Add(new()
                {
                    cObject = hit.collider.gameObject,
                    colliderType = eWallDirection.RIGHT
                });
            }
        }
        {
            RaycastHit[] hits = GetWallCheckObjects(Vector3.forward);
            foreach (var hit in hits)
            {
                _colliderInfos[eWallDirection.FRONT].Add(new()
                {
                    cObject = hit.collider.gameObject,
                    colliderType = eWallDirection.FRONT
                });
            }
        }
        {
            RaycastHit[] hits = GetWallCheckObjects(Vector3.back);
            foreach (var hit in hits)
            {
                _colliderInfos[eWallDirection.BACK].Add(new()
                {
                    cObject = hit.collider.gameObject,
                    colliderType = eWallDirection.BACK
                });
            }
        }
        
        // _rightWallObjs = GetWallCheckObjects(Vector3.right);
        stateMap[_curState].FixedUpdateState();
    }

    private void OnDrawGizmos()
    {
        DrawGroundCheckBox();
        DrawWallCheckBox();
    }

    private void DrawGroundCheckBox()
    {
        Vector3 center = transform.position;
        center.y -= _groundCheckBoxSize.y / 2;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, _groundCheckBoxSize);
    }

    private void DrawWallCheckBox()
    {
        if (TryGetComponent<CapsuleCollider>(out var collider))
        {
            // DrawWallCube();
            Vector3 wallBoxFrontBack = new Vector3(_wallBoxWidth, _wallBoxHeight, _wallBoxThickness);
            {
                var wallFrontPos = collider.center;
                wallFrontPos.z += collider.radius + (wallBoxFrontBack.z / 2);
                wallFrontPos += transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(wallFrontPos, wallBoxFrontBack);
            }

            {
                var wallBackPos = collider.center;
                wallBackPos.z -= collider.radius + (wallBoxFrontBack.z / 2);
                wallBackPos += transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(wallBackPos, wallBoxFrontBack);
            }
            
            Vector3 wallBoxLeftRight = new Vector3(_wallBoxThickness, _wallBoxHeight, _wallBoxWidth);
            {
                var wallLeftPos = collider.center;
                wallLeftPos.x += collider.radius + (wallBoxLeftRight.x / 2);
                wallLeftPos += transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(wallLeftPos, wallBoxLeftRight);
            }
            
            {
                var wallRightPos = collider.center;
                wallRightPos.x -= collider.radius + (wallBoxLeftRight.x / 2);
                wallRightPos += transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(wallRightPos, wallBoxLeftRight);
            }
        }
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
    
    public void MovePosition(Vector3 direction)
    {
        Vector3 moveDirection = GetMoveDirectionVector(direction);

        Vector3 moveVelocity = moveDirection * _moveSpeed;
        Debug.Log($"[testumMove]moveDirection({moveDirection})moveVelocity({moveVelocity})");
        
        _rigidbody.velocity = moveVelocity;
    }

    public Vector3 GetMoveDirectionVector(Vector3 normDirection)
    {
        if (ContactWalls(new[] { eWallDirection.LEFT, eWallDirection.BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT, eWallDirection.FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.back).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT, eWallDirection.BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT, eWallDirection.FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.back).normalized);
        }
        else if (ContactWall(eWallDirection.LEFT))
        {
            normDirection = GetInterpolationWallDirection(normDirection,  Vector3.right);
            // normDirection.x = normDirection.x > 0 ? 0 : normDirection.x;
        }
        else if (ContactWall(eWallDirection.RIGHT))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.left);
            // normDirection.x = normDirection.x < 0 ? 0 : normDirection.x;
        }
        else if (ContactWall(eWallDirection.BACK))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.forward);
            // normDirection.z = normDirection.z > 0 ? 0 : normDirection.z;
        }
        else if (ContactWall(eWallDirection.FRONT))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.back);
            // normDirection.z = normDirection.z < 0 ? 0 : normDirection.z;
        }

        return normDirection;
    }

    private Vector3 GetInterpolationWallDirection(Vector3 normDirection, Vector3 standard)
    {
        Vector3 result = normDirection;
        // string testLog = "";
        // testLog += $"[testumWall]normDirection({result})standard({standard})내적({Vector3.Dot(result, standard)})";
        Vector3[] reverses = GetReverseVectors(standard);
        foreach (var reverse in reverses)
        {
            if (reverse.normalized != result.normalized)
                continue;
            result -= standard * Vector3.Dot(result, standard);
            // testLog += $"changed({result})";
            // Debug.Log(testLog);
        }

        return result.normalized;
    }

    private Vector3[] GetReverseVectors(Vector3 vector)
    {
        return new[] { vector, Quaternion.Euler(0, 45, 0) * vector, Quaternion.Euler(0, -45, 0) * vector };
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

    private RaycastHit[] GetGroundCheckObjects()
    {
        int layerMask = 1;
        layerMask = layerMask << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetGroundBoxCenter(), GetGroundBoxHalfSize(), Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        return hits;
    }
    
    private RaycastHit[] GetWallCheckObjects(Vector3 direction)
    {
        int layerMask = 1;
        layerMask = layerMask << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetWallBoxCenter(direction), GetLeftRightWallBoxSize(), direction, Quaternion.identity, 0f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        Debug.Log($"[testum]wall collision size({hits.Length})");
        return hits;
    }
    
    private RaycastHit[] GetRightWallCheckObjects()
    {
        int layerMask = 1;
        layerMask = layerMask << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetWallBoxCenter(Vector3.right), GetLeftRightWallBoxSize(), Vector3.right, Quaternion.identity, 0f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        Debug.Log($"[testum]wall collision size({hits.Length})");
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
        var rayObjs = _groundObjs;
        if (null == rayObjs)
            return;
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
    
    private Vector3 GetWallBoxCenter(Vector3 normVector)
    {
        if (false == TryGetComponent<CapsuleCollider>(out var collider))
        {
            Debug.LogError($"[testum]캡슐 콜리더 없음!");
            return Vector3.zero;
        }
        Vector3 boxCenter = transform.position + collider.center;   // 캐릭터 캡슐의 중앙
        boxCenter -= normVector * (collider.radius + (_wallBoxThickness / 2));    // 캐릭터 중앙에서 Wall Box 중앙 구하기
        Debug.Log($"[testum]boxCenter({boxCenter})");
            
        // var wallBox = new Vector3(_wallBoxThickness, _wallBoxHeight, _wallBoxWidth);
        return boxCenter;
    }
    
    private bool ContactWalls(eWallDirection[] directions)
    {
        bool result = true;
        foreach (var direction in directions)
        {
            result &= ContactWall(direction);
        }

        return result;
    }

    private bool ContactWall(eWallDirection wallDir)
    {
        if (false == _colliderInfos.ContainsKey(wallDir))
            return false;
        return _colliderInfos[wallDir].Count > 0;
    }
    
    private Vector3 GetLeftRightWallBoxSize()
    {
        return new Vector3(_wallBoxThickness, _wallBoxHeight, _wallBoxWidth) / 2;
    }
}