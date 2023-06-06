using System;
using System.Collections.Generic;
using Animancer;
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
    BACK,
    LEFT_FRONT,
    RIGHT_FRONT,
    LEFT_BACK,
    RIGHT_BACK,
}
[Serializable]
public class ColliderCube
{
    public Vector3 colliderSize;
    public Vector3 colliderPos;
    public Vector3 gizmoPos;
}
[Serializable]
public struct AttackPartData
{
    public AttackRangeType attackPart;
    public AttackCollider attackCollider;
}

// 엄todo : 대쉬 슬라이드로 언덕 오르기/내리기 테스트 및 구현 >>>> 보류
// // 대각 바닥 오르기 안되는 버그
// // >> 원인 : 바닥 타입 일 때 posY 갱신으로 인해서 대각 오르기 실패 >> slop 타입, wall 타입 분리 필요
// // 대각 바닥 내리기 기능 개발
// // >> slop 타입 분리 필요
// // >> 바닥 콜리전 없을 때, 경사용 컬리전으로 다시 체크하는 방식?
// 엄todo : 공격 기능 개발
// // - 피격 시 동일한 AttackCollider 인스턴스일 때 무시 처리
// 엄todo : 서버가 붙으면 어떻게 위치에 대한 보간을 처리할지
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

    [Header("Move Speed Stats")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _moveSpeed = 0.0f;
    [Header("Jump Stats")]
    [SerializeField] private AnimationCurve _jumpUp = new ();
    [SerializeField] private AnimationCurve _jumpDown = new ();
    [SerializeField] private bool _useReverse = false;
    [SerializeField] private float _jumpMaxHeight = 2f;
    [SerializeField] private float _jumpUpMaxTimer = 2f;
    [SerializeField] private float _jumpDownMaxTimer = 1f;
    
    [Header("Ground Collider")]
    [SerializeField] private ColliderCube _groundCollider = new ColliderCube
    {
        colliderSize = new(0.2f, 0.03f, 0.2f),
        colliderPos = default,
        gizmoPos = default
    };
    [Header("Wall Collider")]
    [SerializeField] private ColliderCube _wallCollider = new ColliderCube
    {
        colliderSize = new(0.1f, 1.5f, 0.05f),
        colliderPos = default,
        gizmoPos = default
    };
    [SerializeField] public float _wallBoxThickness = 0.05f;
    [SerializeField] public float _wallBoxWidth = 0.1f;
    [SerializeField] public float _wallBoxHeight = 1.5f;
    
    [Header("Hit Collider")]
    [SerializeField] private List<HitCollider> _hitColliders = new();
    private Dictionary<HitColliderType, HitCollider> _hitColliderMap = new();
    
    [Header("Attack Collider")]
    [SerializeField] private List<AttackPartData> _attackPartDatas = new();
    private Dictionary<AttackRangeType, AttackCollider> _attackColliderMap = new();

    [Header("3D Phygics Component")] 
    [SerializeField] private Rigidbody _rigidbody;

    [Header("UI")]
    protected Slider slider;

    private float _curHp = 0f;
    protected float sliderScale = 1.0f;

    protected Vector3 _directionVector = Vector3.zero;

    protected Dictionary<eState, State> _stateMap = new();

    [SerializeField] protected eState _prevState;
    [SerializeField] protected eState _curState;

    public bool _isGround = false;
    private bool _checkGround = true;

    private List<StateInfo> _changeStates = new();

    private Tween _rotationTween = null;

    private RaycastHit[] _groundObjs = null;
    private Dictionary<eWallDirection, List<ColliderInfo>> _colliderInfos = new();
    private RaycastHit[] _leftWallObjs = null;
    private RaycastHit[] _rightWallObjs = null;
    private RaycastHit[] _frontWallObjs = null;
    private RaycastHit[] _backWallObjs = null;

    protected MoveSet _moveSet = new();
    protected AnimancerComponent _animancer;
    
    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
        MakeFixedDeltaTimeCurve(_jumpUp, _jumpUpMaxTimer);
        if (false == _useReverse)
            MakeFixedDeltaTimeCurve(_jumpDown, _jumpDownMaxTimer);
        else
        {
            MakeReverseCurve(_jumpUp, _jumpDown);
        }

        foreach (var partData in _attackPartDatas)
        {
            partData.attackCollider.SetOwner(this);
            if (false == _attackColliderMap.ContainsKey(partData.attackPart))
                _attackColliderMap.Add(partData.attackPart, partData.attackCollider);
        }

        foreach (var hitCollider in _hitColliders)
        {
            var hitColliderType = hitCollider.GetHitType();
            if (false == _hitColliderMap.ContainsKey(hitColliderType))
            {
                hitCollider.gameObject.SetActive(false);
                _hitColliderMap.Add(hitCollider.GetHitType(), hitCollider);
            }
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
        UpdateWallCollisions(eWallDirection.LEFT, Vector3.left);
        UpdateWallCollisions(eWallDirection.RIGHT, Vector3.right);
        UpdateWallCollisions(eWallDirection.FRONT, Vector3.forward);
        UpdateWallCollisions(eWallDirection.BACK, Vector3.back);
        UpdateWallCollisions(eWallDirection.LEFT_FRONT, (Vector3.left + Vector3.forward).normalized);
        UpdateWallCollisions(eWallDirection.RIGHT_FRONT, (Vector3.right + Vector3.forward).normalized);
        UpdateWallCollisions(eWallDirection.LEFT_BACK, (Vector3.left + Vector3.back).normalized);
        UpdateWallCollisions(eWallDirection.RIGHT_BACK, (Vector3.right + Vector3.back).normalized);

        // _rightWallObjs = GetWallCheckObjects(Vector3.right);
        _stateMap[_curState].FixedUpdateState();
    }

    private void UpdateWallCollisions(eWallDirection eWallDir, Vector3 dirVector)
    {
        _colliderInfos.Add(eWallDir, new());
        {
            RaycastHit[] hits = GetWallCheckObjects(dirVector);
            foreach (var hit in hits)
            {
                _colliderInfos[eWallDir].Add(new()
                {
                    cObject = hit.collider.gameObject,
                    colliderType = eWallDir
                });
            }
            Debug.Log($"[testum][WallCollision][{eWallDir}]count({hits.Length})");
        }
    }

    private void OnDrawGizmos()
    {
        DrawGroundCheckBox();
        DrawWallCheckBox();
    }

    private void DrawGroundCheckBox()
    {
        Vector3 center = new(0f, - _groundCollider.colliderSize.y / 2, 0f);
        _groundCollider.gizmoPos = center;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, _groundCollider.colliderSize);
    }

    private void DrawWallCheckBox()
    {
        if (TryGetComponent<CapsuleCollider>(out var collider))
        {
            // DrawWallCube();
            // Vector3 wallBoxFrontBack = new Vector3(_wallBoxWidth, _wallBoxHeight, _wallBoxThickness);
            // {
            //     var boxPos = transform.position + collider.center;
            //     var pivot = 0.5f;
            //     var pivotPos = wallBoxFrontBack.z / 2;
            //     var centerMovePos = collider.radius;
            //     boxPos.z += centerMovePos + pivotPos;
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawWireCube(boxPos, wallBoxFrontBack);
            // }
            for (int i = 0; i < 8; i++)
            {
                var rotateAngle = i * 45;
                var characterCenterPos = transform.position + collider.center; 
                var pivot = 0.5f;
                var pivotPos = _wallCollider.colliderSize.z / 2;
                Gizmos.matrix = Matrix4x4.TRS(characterCenterPos, Quaternion.Euler(0, rotateAngle, 0), Vector3.one);
                Gizmos.color = Color.red;
                var cubePos = Vector3.forward * (collider.radius + _wallCollider.colliderSize.z / 2);
                Gizmos.DrawWireCube(cubePos, _wallCollider.colliderSize);
            }

            // {
            //     var wallBackPos = collider.center;
            //     wallBackPos.z -= collider.radius + (wallBoxFrontBack.z / 2);
            //     wallBackPos += transform.position;
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawWireCube(wallBackPos, wallBoxFrontBack);
            // }
            //
            // Vector3 wallBoxLeftRight = new Vector3(_wallBoxThickness, _wallBoxHeight, _wallBoxWidth);
            // {
            //     var wallLeftPos = collider.center;
            //     wallLeftPos.x += collider.radius + (wallBoxLeftRight.x / 2);
            //     wallLeftPos += transform.position;
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawWireCube(wallLeftPos, wallBoxLeftRight);
            // }
            //
            // {
            //     var wallRightPos = collider.center;
            //     wallRightPos.x -= collider.radius + (wallBoxLeftRight.x / 2);
            //     wallRightPos += transform.position;
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawWireCube(wallRightPos, wallBoxLeftRight);
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log($"[testum]clip name : {_moveSet.GetCurActionName()}");
        if (_changeStates.Count > 0)
        {
            // 엄todo : 이 시점에 _curState를 queue에 저장하면 되겠지? queue data클래스는 {이전 프레임 시간, List<StateInfo>} 이렇게 구성하면 될듯?
            Debug.Log($"[testState]Change prev({_curState}) count({_changeStates.Count})");
            if (_changeStates.Count == 1)
            {
                eState state = _changeStates[0].state;
                _stateMap[_curState].EndState();
                _stateMap[state].StartState();
                _prevState = _curState;
                _curState = state;
            }
            else
            {
                eState state = SelectNextState();
                _stateMap[_curState].EndState();
                _stateMap[state].StartState();
                _prevState = _curState;
                _curState = state;
            }
            _changeStates.Clear();
        }
        _stateMap[_curState].UpdateState();
    }

    private void LateUpdate()
    {
        UpdateUI();
    }

    private eState SelectNextState()
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
    
    public eState GetCurState()
    {
        return _curState;
    }
    
    public MoveSet GetMoveSet()
    {
        return _moveSet;
    }
    
    public void MovePosition(Vector3 direction)
    {
        Vector3 moveDirection = GetMoveDirectionVector(direction);

        Vector3 moveVelocity = moveDirection * _moveSpeed;
        Debug.Log($"[testumMove]moveDirection({moveDirection})moveVelocity({moveVelocity})");
        
        _rigidbody.velocity = moveVelocity;
    }
    
    public void MovePosition(Vector3 direction, float moveSpeed)
    {
        Vector3 moveDirection = GetMoveDirectionVector(direction);

        Vector3 moveVelocity = moveDirection * moveSpeed;
        Debug.Log($"[testumMove]moveDirection({moveDirection})moveVelocity({moveVelocity})");
        
        _rigidbody.velocity = moveVelocity;
    }

    public Vector3 GetMoveDirectionVector(Vector3 normDirection)
    {
        // ContactWallForLog(new[] { eWallDirection.LEFT, eWallDirection.RIGHT, eWallDirection.FRONT, eWallDirection.BACK });
        // 3개의 충돌체 검증 시 정확한 역방향 처리
        if (ContactWalls(new[] { eWallDirection.LEFT, eWallDirection.LEFT_BACK, eWallDirection.BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT, eWallDirection.LEFT_FRONT, eWallDirection.FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.back).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT, eWallDirection.RIGHT_BACK, eWallDirection.BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT, eWallDirection.RIGHT_FRONT, eWallDirection.FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.back).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT_BACK, eWallDirection.LEFT, eWallDirection.LEFT_FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection,  Vector3.right);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT_BACK, eWallDirection.RIGHT, eWallDirection.RIGHT_FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.left);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT_BACK, eWallDirection.BACK, eWallDirection.RIGHT_BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.forward);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT_FRONT, eWallDirection.FRONT, eWallDirection.RIGHT_FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.back);
        }
        // 2개의 충돌체 검출 시 정확한 역방향 처리
        else if (ContactWalls(new[] { eWallDirection.LEFT, eWallDirection.LEFT_BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT_BACK, eWallDirection.BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT, eWallDirection.LEFT_FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.back).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.LEFT_FRONT, eWallDirection.FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.right + Vector3.back).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT, eWallDirection.RIGHT_BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT_BACK, eWallDirection.BACK }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.forward).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT, eWallDirection.RIGHT_FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.back).normalized);
        }
        else if (ContactWalls(new[] { eWallDirection.RIGHT_FRONT, eWallDirection.FRONT }))
        {
            normDirection = GetInterpolationWallDirection(normDirection, (Vector3.left + Vector3.back).normalized);
        }
        // 1개의 충돌체 검출 시 정확한 역방향 처리
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
        // OnHit(other);
    }

    public void OnHit(Collider other)
    {
        // 피해 받았을때 진입
        // other : attacker
        // name : defender
        if (other.TryGetComponent<AttackCollider>(out var attackCollider))
        {
            Debug.Log($"[testum][name:{name}]be hit other({other.name})");
            var attacker = attackCollider.GetOwner();
            if (attacker != this)
            {
                AttackType attackType = attackCollider.GetAttackType();
                switch (attackType)
                {
                    case AttackType.NONE:
                        break;
                    case AttackType.NORMAL:
                        // 엄todo: isGround 및 피격 여부로 체크 변경하기
                        if (_curState == eState.AIRBORNE_DAMAGED
                            || _curState == eState.DAMAGED_AIRBORNE_LOOP
                            || _curState == eState.DAMAGED_LANDING)
                            ChangeState(eState.AIRBORNE_DAMAGED);
                        else
                        {
                            ChangeState(eState.NORMAL_DAMAGED);
                        }
                        break;
                    case AttackType.AIRBORNE:
                        ChangeState(eState.AIRBORNE_DAMAGED);
                        break;
                }
            }
        }
        // Character attacker = other.transform.parent.GetComponent<Character>();
        // if (attacker == null)
        // {
        //     Debug.Log("Attacker is not character.");
        //     return;
        // }
        //
        // _curHp -= attacker.getAttackDamage();
        // if (_curHp <= 0f)
        // {
        //     _curHp = 0f;
        //     ChangeState(eState.DEAD);
        // }
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

    public RaycastHit[] GetGroundCheckObjects()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetGroundBoxCenter(), _groundCollider.colliderSize / 2, Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        return hits;
    }
    
    private RaycastHit[] GetWallCheckObjects(Vector3 direction)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        
        if (false == TryGetComponent<CapsuleCollider>(out var collider))
            return null;
        var from = Vector3.forward;
        var to = direction;
        float rotateAngle = Vector3.SignedAngle(from, to, transform.up);
        var characterCenterPos = transform.position + collider.center;
        var wallPos = direction * (collider.radius + _wallCollider.colliderSize.z / 2);
        var boxCenter = Matrix4x4.TRS(characterCenterPos, Quaternion.Euler(0, rotateAngle, 0), Vector3.one).GetPosition();
        // Debug.Log($"[testum]cobj direction({direction}) ({GetWallBoxCenter(direction)}) boxCenter({boxCenter-wallPos})");
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter-wallPos, _wallCollider.colliderSize / 2, direction, Quaternion.identity, 0f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        return hits;
    }
    
    private RaycastHit[] GetWallFrontObjects(Vector3 direction)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        
        // GetWallBoxCenter 대신 각도의 값으로 바꾸자 일단 졸려서 못함
        if (false == TryGetComponent<CapsuleCollider>(out var collider))
            return null;
        
        var from = Vector3.forward;
        var to = direction;
        float rotateAngle = Vector3.SignedAngle(from, to, transform.up);
        Debug.Log($"[testum]Rotate({rotateAngle})");
        
        var characterCenterPos = transform.position + collider.center;
        var boxCenter = Matrix4x4.TRS(characterCenterPos, Quaternion.Euler(0, rotateAngle, 0), Vector3.one).GetPosition();
        
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, _wallCollider.colliderSize / 2, Vector3.forward, Quaternion.identity, 0f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        Debug.Log($"[testum][{direction}]wall collision size({hits.Length})");
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
        if (rayObjs.Length > 1)
            Debug.Log($"[testum]curState({_curState}) rayObj count({rayObjs.Length})");
        foreach (var rayObj in rayObjs)
        {
            if (rayObj.transform.TryGetComponent<Ground>(out var ground))
            {
                if (rayObjs.Length > 1)
                    Debug.Log($"[testum]curState({_curState}) rayObj height({ground.heightPosY}) curHeightPosY({transform.position})");
                var changeHeightPosY = ground.heightPosY - transform.position.y;
                if (groundHeight < ground.heightPosY && changeHeightPosY < 0.2f)
                {
                    groundHeight = ground.heightPosY;
                    transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
                }
            }
        }
    }

    private Vector3 GetGroundBoxHalfSize()
    {
        return _groundCollider.colliderSize / 2;
    }
    
    private Vector3 GetGroundBoxCenter()
    {
        Vector3 boxCenter = transform.position;
        boxCenter.y -= _groundCollider.colliderSize.y / 2;
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
    
    private void ContactWallForLog(eWallDirection[] directions)
    {
        string log = "[testum][Logger]";
        foreach (var direction in directions)
        {
            log += $"{direction}({ContactWall(direction)})";
        }
        Debug.Log(log);
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

    public void ActiveAttackColliders(bool enable)
    {
        foreach (var partData in _attackPartDatas)
        {
            partData.attackCollider.gameObject.SetActive(enable);
        }
    }
    public void ActiveAttackCollider(bool enable, AttackRangeType colliderType, AttackType attackType = AttackType.NORMAL)
    {
        if (false == _attackColliderMap.ContainsKey(colliderType))
            return;
        _attackColliderMap[colliderType].gameObject.SetActive(enable);
        _attackColliderMap[colliderType].SetAttackType(attackType);
    }
    
    public void ActiveHitCollider(bool enable, HitColliderType colliderType)
    {
        if (false == _hitColliderMap.ContainsKey(colliderType))
            return;
        _hitColliderMap[colliderType].gameObject.SetActive(enable);
    }
    
    private Vector3 GetLeftRightWallBoxSize()
    {
        return new Vector3(_wallBoxThickness, _wallBoxHeight, _wallBoxWidth) / 2;
    }
}