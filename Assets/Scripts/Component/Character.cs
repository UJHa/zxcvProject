using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Animancer;
using DataClass;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class StateInfo
{
    public eRoleState state;
    public eStateType stateType;
}

// int 값으로 더 클 수록 더 강제하여 실행하도록 State 룰 설정
public enum eStateType
{
    NONE,
    DAMAGE_LANDING,
    INPUT,
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
    public Vector3 Size;
    public Vector3 gizmoPos;
}
[Serializable]
public struct AttackPartData
{
    public HitboxType attackPart;
    public AttackCollider attackCollider;
}

public enum ColliderType
{
    NONE,
    LEFT_HAND,
    RIGHT_HAND,
    LEFT_FOOT,
    RIGHT_FOOT
}

[Serializable]
public struct PartColliderData
{
    public ColliderType Type;
    public PartCollider Collider;
}

// 엄todo : 대쉬 슬라이드로 언덕 오르기/내리기 테스트 및 구현 >>>> 보류
// // 대각 바닥 오르기 안되는 버그
// // >> 원인 : 바닥 타입 일 때 posY 갱신으로 인해서 대각 오르기 실패 >> slop 타입, wall 타입 분리 필요
// // 대각 바닥 내리기 기능 개발
// // >> slop 타입 분리 필요
// // >> 바닥 콜리전 없을 때, 경사용 컬리전으로 다시 체크하는 방식?
// 피격 시 동일한 AttackCollider 인스턴스일 때 무시 처리
// 엄todo : 서버가 붙으면 어떻게 위치에 대한 보간을 처리할지
public class Character : MonoBehaviour
{
    [Header("[ Test Speed ]")]
    [SerializeField] private Vector3 curVelocity = Vector3.zero;
    [Header("[ Test Bot Trace]")]
    [SerializeField] private float _walkTraceDistance = 1f;
    [SerializeField] private float _attackStartDistance = 4f;
    [Header("[ Stats ]")]
    [SerializeField] private float _fullHp = 100f;
    [SerializeField] private float _curHp = 0f;
    [SerializeField] private float _hp = 5f;
    [SerializeField] private float _mp = 5f;
    [SerializeField] private float _strength = 5f;
    [SerializeField] private float _agility = 6f;
    [SerializeField] private float _intellect = 5f;
    [SerializeField] private string _statKey = "Default";
    
    [Header("[ Anim CrossFadeTime ]")]
    [SerializeField] private string animFadeInfoKey = "Default";

    [Header("[ Move Speed Stats ]")]
    [SerializeField] private float _walkSpeedRatio = 0.3f;
    [SerializeField] private float _runSpeedRatio = 1f;
    [SerializeField] private float _moveSpeed = 0.0f;
    [Header("[ Jump Stats ]")]
    [SerializeField] private float _jumpMaxHeight = 2f;
    [SerializeField] private float _jumpUpMaxTimer = 0.8f;
    [SerializeField] private float _jumpDownMaxTimer = 0.6f;
    [SerializeField] private float _airboneUpMaxTimer = 0.8f;
    [SerializeField] private float _airboneDownMaxTimer = 0.6f;
    
    [SerializeField] private float _gravityDownTime = 0.6f;
    [SerializeField] private float _gravityDownHeight = 2f;
    
    [Header("[ Ground Collider ]")]
    [SerializeField] private ColliderCube _groundCollider = new ColliderCube
    {
        Size = new(0.2f, 0.03f, 0.2f),
        gizmoPos = default
    };
    [Header("[ Wall Collider ]")]
    [SerializeField] private ColliderCube _wallCollider = new ColliderCube
    {
        Size = new(0.1f, 1.5f, 0.05f), // (width, height, thinckness)
        gizmoPos = default
    };
    
    [Header("[ RootPrefab ]")]
    [SerializeField] private Transform _rootTransform;

    [Header("[ Hit Collider ]")]
    [SerializeField] private List<HitCollider> _hitColliders = new();
    private Dictionary<HitColliderType, List<HitCollider>> _hitColliderMap = new();
    
    [Header("[ Attack Collider ]")]
    [FormerlySerializedAs("_attackPartDatas")]
    [SerializeField] private List<AttackPartData> _attackCollisionRangeDatas = new();
    private Dictionary<HitboxType, AttackCollider> _attackCollisionRangeMap = new();
    
    [Header("[ Attack Collider ]")]
    [FormerlySerializedAs("_partColliderDatas")]
    [SerializeField] private List<PartColliderData> _equipPartColliderDatas = new();
    private Dictionary<ColliderType, PartCollider> _equipPartColliderMap = new();

    [Header("[ 3D Phygics Component ]")] 
    [SerializeField] private Rigidbody _rigidbody;

    protected Vector3 _directionVector = Vector3.zero;
    protected Vector3 _damagedDirectionVector = Vector3.zero;
    
    protected Dictionary<eRoleState, State> _roleStateMap = new();
    
    [SerializeField] protected eRoleState _prevRoleState;
    [SerializeField] protected eRoleState _curRoleState;
    [SerializeField] protected eRole _curRole = eRole.FIGHTER;
    
    // 엄todo : 캐릭터가 날려야할 투사체 정보를 어떻게 가질 지 고민 후 정리
    [SerializeField] protected GameObject _projectilePos;

    protected DrawDebugCharacter _drawDebug;
    protected HumanBoneInfo _humanBoneInfo = new();

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

    private List<Collider> _onHitQueue = new();

    protected float _attackedMaxHeight = 0f;
    protected float _attackedAirborneUpTime = 0f;

    // 엄todo : 이 Fx를 미리 로드하기 위한 클래스나 시스템이 어디에 들어가야 할지 고민하기
    private GameObject hitFx;
    
    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out var rigidbody))
            _rigidbody = rigidbody;
        
        _drawDebug = new();
        _drawDebug.Init(this);
        _drawDebug.SetGroundCollider(_groundCollider);
        _drawDebug.SetWallCollider(_wallCollider);
        
        if (TryGetComponent<AnimancerComponent>(out var animancer))
            _animancer = animancer;

        _humanBoneInfo.Init(_animancer.Animator);
        
        // Equip Helmet 테스트 코드(엄todo : 작업 완료 후 지울것)
        // TestHelmetEquip();

        SettingProjectilePos();

        foreach (var partData in _attackCollisionRangeDatas)
        {
            partData.attackCollider.SetOwner(this);
            if (false == _attackCollisionRangeMap.ContainsKey(partData.attackPart))
                _attackCollisionRangeMap.Add(partData.attackPart, partData.attackCollider);
        }

        foreach (var hitCollider in _hitColliders)
        {
            var hitColliderType = hitCollider.GetHitType();
            hitCollider.gameObject.SetActive(false);
            if (false == _hitColliderMap.ContainsKey(hitColliderType))
                _hitColliderMap.Add(hitColliderType, new());
            _hitColliderMap[hitColliderType].Add(hitCollider);
        }

        foreach (var partColliderData in _equipPartColliderDatas)
        {
            // 엄todo : 현재 미사용 로우폴리 캐릭터 파츠 장착 시 다시 개발 필요
            partColliderData.Collider.SetOwner(this);
            if (false == _equipPartColliderMap.ContainsKey(partColliderData.Type))
                _equipPartColliderMap.Add(partColliderData.Type, partColliderData.Collider);
        }

        InitStats();
        hitFx = Resources.Load<GameObject>("Prefabs/StatusFx/Hits/Hit_01");

        InitStates();
        StartUI();
    }

    private void SettingProjectilePos()
    {
        var obj = Resources.Load<GameObject>("Prefabs/Projectile/ProjectilePos");
        _projectilePos = Instantiate(obj, transform.Find("RotateMain"));
    }

    private void InitStates()
    {
        SettingRoleState("Default", true);
    }
    
    public struct RoleStateData
    {
        public eRoleState roleState;
        public ActionKey actionKey;
        public Type stateClassType;
    }

    protected Dictionary<string, RoleStateData> _defaultDataMap = new();

    public void SettingRoleState(string roleStateDataName, bool isDefault = false)
    {
        var data = CharacterRoleStateTable.GetData(roleStateDataName);
        var properties = typeof(CharacterRoleStateData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Debug.Log($"[testRoleTable]length({properties.Length})");
        foreach (var pInfo in properties)
        {
            var value = pInfo.GetValue(data);
            if (value is string[] valueStr && 2 == valueStr.Length)
            {
                var roleState = UmUtil.StringToEnum<eRoleState>(Regex.Replace(pInfo.Name, @"(\p{Ll})(\p{Lu})", "$1_$2").ToUpper());
                var actionKey = UmUtil.StringToEnum<ActionKey>(valueStr[1]);
                var stateClassType = Type.GetType(valueStr[0]);
                if (isDefault)
                {
                    _defaultDataMap[pInfo.Name] = new()
                    {
                        roleState = roleState,
                        actionKey = actionKey,
                        stateClassType = stateClassType
                    };
                }
                else
                {
                    actionKey = actionKey == default ? _defaultDataMap[pInfo.Name].actionKey : actionKey;
                    stateClassType = stateClassType == default ? _defaultDataMap[pInfo.Name].stateClassType : stateClassType;
                }
                bool isSuccess = RegisterRoleState(roleState, actionKey, stateClassType);
                Debug.Log($"[setRoleState]{roleState}:[{actionKey}][{stateClassType}][{isSuccess}]");
            }
        }
    }

    private void TestHelmetEquip()
    {
        // 헬멧 장착 코드
        if (name.Contains("PlayerMain"))
        {
            // 엄todo 헬멧 장착할 소켓 transform은 character가 가지도록 구조 개선 필요
            Transform headSlot = null;
            List<Transform> tfmRootChilds = UmUtil.GetAllChildList(_rootTransform);
            foreach (var prefabTfm in tfmRootChilds)
            {
                if (prefabTfm.name.Equals("HeadEnd_M"))
                    headSlot = prefabTfm;
            }
            
            var armorHelmet = Resources.Load<MeshFilter>("Prefabs/Armor/ArmorHelmet");
            armorHelmet = Instantiate(armorHelmet, headSlot);
            Mesh helmetMesh = armorHelmet.mesh;
            List<Vector3> verticals = new();
            for (int i = 0; i < helmetMesh.vertexCount; i++)
            {
                Debug.Log($"[rappingHelmet]helmetMesh vertical[{i}]({helmetMesh.vertices[i]})");
                Vector3 vertice = helmetMesh.vertices[i];
                vertice.y -= helmetMesh.bounds.center.y;
                verticals.Add(vertice);
            }
            helmetMesh.SetVertices(verticals);
            Bounds bounds = new();
            bounds = helmetMesh.bounds;
            bounds.center = Vector3.zero;
            bounds.extents = Vector3.zero;
            helmetMesh.bounds = bounds;
        }
    }

    protected void InitStats()
    {
        // 엄todo 스탯 정보는 _statKey 기반으로 가져오도록 변경하기(각 Player가 수정한 값 기준)
        var statData = GetStatData();
        _hp = statData.health;
        _mp = statData.mana;
        _strength = statData.strength;
        _agility = statData.agility;
        _intellect = statData.intellect;
        CalculateStats();
    }

    public void CalculateStats()
    {
        _fullHp = _hp * 20f;
        // stats 변경에 따른 데이터 연산용 함수
        _curHp = _fullHp;
    }
    
    public float GetJumpUpVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("jumpUp"), curDeltatime, xMaxValue, yMaxValue);
    }
    
    public float GetJumpDownVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("jumpDown"), curDeltatime, xMaxValue, yMaxValue);
    }
    
    public float GetAirBoneUpVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("airBoneUp"), curDeltatime, xMaxValue, yMaxValue);
    }
    
    public float GetAirBoneDownVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("airBoneDown"), curDeltatime, xMaxValue, yMaxValue);
    }
    
    public float GetKnockBackVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("knockBack"), curDeltatime, xMaxValue, yMaxValue);
    }
    
    public float GetFlyAwayGroundVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("flyAwayGround"), curDeltatime, xMaxValue, yMaxValue);
    }
    
    public float GetFlyAwayHeightVelocity(float curDeltatime, float xMaxValue, float yMaxValue)
    {
        return GameManager.Instance.GetCurveVelocity(GameManager.Instance.GetAnimCurve("flyAwayHeight"), curDeltatime, xMaxValue, yMaxValue);
    }

    private void StartUI()
    {
        if (null != UIManagerInGame.Instance)
            UIManagerInGame.Instance.hudManager.AddPlayerSlider(GetHashCode(), this);
    }

    private void FixedUpdate()
    {
        // ground check
        RefreshGroundCheckObjects();
        
        _colliderInfos.Clear();
        UpdateWallCollisions(eWallDirection.LEFT, Vector3.left);
        UpdateWallCollisions(eWallDirection.RIGHT, Vector3.right);
        UpdateWallCollisions(eWallDirection.FRONT, Vector3.forward);
        UpdateWallCollisions(eWallDirection.BACK, Vector3.back);
        UpdateWallCollisions(eWallDirection.LEFT_FRONT, (Vector3.left + Vector3.forward).normalized);
        UpdateWallCollisions(eWallDirection.RIGHT_FRONT, (Vector3.right + Vector3.forward).normalized);
        UpdateWallCollisions(eWallDirection.LEFT_BACK, (Vector3.left + Vector3.back).normalized);
        UpdateWallCollisions(eWallDirection.RIGHT_BACK, (Vector3.right + Vector3.back).normalized);
        
        _roleStateMap[_curRoleState].FixedUpdateState();
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
        }
    }

    private void OnDrawGizmos()
    {
        _drawDebug?.DrawUpdate();
    }
    
    public void ClearAttackInfoData()
    {
        _attackedMaxHeight = 0f;
        _attackedAirborneUpTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_changeStates.Count > 0)
        {
            // 엄todo : 이 시점에 _curState를 queue에 저장하면 되겠지? queue data클래스는 {이전 프레임 시간, List<StateInfo>} 이렇게 구성하면 될듯?
            Debug.Log($"[testState]Change prev({_curRoleState}) count({_changeStates.Count})");
            eRoleState state = eRoleState.NONE;
            if (_changeStates.Count == 1)
                state = _changeStates[0].state;
            else
                state = SelectNextState();
            _roleStateMap[_curRoleState].EndState();
            _roleStateMap[state].StartState();
            _prevRoleState = _curRoleState;
            _curRoleState = state;
            _changeStates.Clear();
        }
        _roleStateMap[_curRoleState].UpdateState();

        while (_onHitQueue.Count > 0)
        {
            var hitboxCollider = _onHitQueue[0];
            if (hitboxCollider.TryGetComponent<AttackCollider>(out var attackCollider))
            {
                var attacker = attackCollider.GetOwner();
                if (attacker != this)
                    ProcessHit(attackCollider);
            }
            _onHitQueue.RemoveAt(0);
        }
    }
    
    private void ProcessHit(AttackCollider attackCollider)
    {
        var attacker = attackCollider.GetOwner();
        AttackInfoData attackInfo = attackCollider.GetAttackInfo();
        AttackType attackType = UmUtil.StringToEnum<AttackType>(attackInfo.attackType);
        _attackedMaxHeight = attackInfo.airborneHeight;
        _attackedAirborneUpTime = attackInfo.airborneTime;
        var damage = attackInfo.damageRatio * attacker._strength;
        SetDamage(damage);
        var closePos = attackCollider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        var hitFxObj = Instantiate(hitFx);
        hitFxObj.transform.position = closePos;
        if (attackCollider.TryGetComponent<Projectile>(out var projectile))
        {
            Destroy(projectile.gameObject);
        }
        switch (attackType)
        {
            case AttackType.NONE:
                break;
            case AttackType.NORMAL:
                // Debug.Log($"[{name}]Attacked attackername({attacker.name})({hitboxKey})({curHitboxKey}) State({attacker._curState})");
                // 엄todo: isGround 및 피격 여부로 체크 변경하기
                if (!IsGround())
                    ChangeState(eRoleState.AIRBORNE_DAMAGED);
                else
                {
                    if (IsDead())
                        ChangeState(eRoleState.DEAD);
                    else
                        ChangeState(eRoleState.NORMAL_DAMAGED);
                }
                break;
            case AttackType.AIRBORNE:
                // 방향을 때린 상대의 방향으로 회전시키기
                RotateToPosition(attacker.transform.position);
                ChangeState(eRoleState.AIRBORNE_DAMAGED);
                break;
            case AttackType.AIR_POWER_DOWN:
                ChangeState(eRoleState.AIRBORNE_POWER_DOWN_DAMAGED);
                break;
            case AttackType.KNOCK_BACK:
                RotateToPosition(attacker.transform.position);
                SetDamagedDirectionVector(attacker.GetDirectionVector());
                Debug.Log($"[attackerDirection]{attacker.GetDirectionVector()}");
                ChangeState(eRoleState.KNOCK_BACK_DAMAGED);
                break;
            case AttackType.FLY_AWAY:
                RotateToPosition(attacker.transform.position);
                SetDamagedDirectionVector(attacker.GetDirectionVector());
                Debug.Log($"[attackerDirection]{attacker.GetDirectionVector()}");
                ChangeState(eRoleState.FLY_AWAY_DAMAGED);
                break;
        }
    }
    
    private eRoleState SelectNextState()
    {
        int layer = (int)eStateType.NONE;
        List<eRoleState> enableStates = new();
        foreach (var stateInfo in _changeStates)
        {
            if ((int)stateInfo.stateType == layer)
                enableStates.Add(stateInfo.state);
            else if ((int)stateInfo.stateType > layer)
            {
                layer = (int)stateInfo.stateType;
                enableStates.Clear();
                enableStates.Add(stateInfo.state);
            }
        }

        return enableStates[0];
    }
    
    public eRoleState GetPrevState()
    {
        return _prevRoleState;
    }
    
    public eRoleState GetCurState()
    {
        return _curRoleState;
    }
    
    public eRoleState GetPrevRoleState()
    {
        return _prevRoleState;
    }
    
    public MoveSet GetMoveSet()
    {
        return _moveSet;
    }

    public Vector3 ComputeMoveVelocityXZ(Vector3 direction)
    {
        Vector3 moveDirection = GetMoveDirectionVector(direction);
        Vector3 moveVelocity = moveDirection * _moveSpeed;
        return moveVelocity;
    }

    public Vector3 GetMoveDirectionVector(Vector3 normDirection)
    {
        // ContactWallForLog(new[] { eWallDirection.LEFT, eWallDirection.RIGHT, eWallDirection.FRONT, eWallDirection.BACK });
        // 3 방향의 충돌체 검증 시 정확한 역방향 처리
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
        // 2 방향의 충돌체 검출 시 정확한 역방향 처리
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
        // 1 방향의 충돌체 검출 시 정확한 역방향 처리
        else if (ContactWall(eWallDirection.LEFT))
        {
            normDirection = GetInterpolationWallDirection(normDirection,  Vector3.right);
        }
        else if (ContactWall(eWallDirection.RIGHT))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.left);
        }
        else if (ContactWall(eWallDirection.BACK))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.forward);
        }
        else if (ContactWall(eWallDirection.FRONT))
        {
            normDirection = GetInterpolationWallDirection(normDirection, Vector3.back);
        }

        return normDirection;
    }

    // 엄todo : 여태 모든 문제를 PysicsMaterial로 해결됨... 타 Util 생성 후 필요한 내용 뽑고 로직 제거하기
    private Vector3 GetInterpolationWallDirection(Vector3 normDirection, Vector3 standard)
    {
        string log = "";
        float angle = Vector3.SignedAngle(Vector3.forward, normDirection, Vector3.up);
        angle = Mathf.RoundToInt(angle / 45f) * 45;
        log += $"[testAngle][{name}]기존 벡터 각도({angle})({normDirection})\n";
        Vector3 result = normDirection;
        // Vector3[] reverses = GetReverseVectors(standard);
        // foreach (var reverse in reverses)
        // {
        //     float rAngle = Vector3.SignedAngle(Vector3.forward, reverse, Vector3.up);
        //     rAngle = Mathf.Round(rAngle);
        //     log += $"리버스 각도({rAngle})";
        //     // if (reverse.normalized != result.normalized)
        //     if (rAngle != angle)
        //     {
        //         log += $"리버스 실패!({rAngle})({angle})({reverse.normalized})({result.normalized})\n";
        //         continue;
        //     }
        //     log += $"리버스 실행!({rAngle})({angle})({reverse.normalized})({result.normalized})\n";
        //     result -= standard * Vector3.Dot(result, standard);
        // }
        // Debug.Log($"{log}");
        result = result.normalized;

        return result;
    }

    private Vector3[] GetReverseVectors(Vector3 vector)
    {
        return new[] { vector, Quaternion.Euler(0, 45, 0) * vector, Quaternion.Euler(0, -45, 0) * vector };
    }

    public void SetDirectionByVector3(Vector3 argVector, float rotateTime = 0.1f)
    {
        _directionVector = argVector;
        var rotateMain = transform.Find("RotateMain");
        // var euler = GetEuler(_directionVector);
        if (null != _rotationTween)
            _rotationTween.Kill();
        var rot = GetRotation(_directionVector);
        _rotationTween = rotateMain.DORotateQuaternion(rot, rotateTime);
    }
    
    public Vector3 GetDirectionVector()
    {
        return _directionVector;
    }

    void SetDamage(float damage)
    {
        _curHp -= damage;
        UIManagerInGame.Instance.hudManager.SetSliderValue(GetHashCode(), _curHp / _fullHp);
        if (IsDead())
        {
            _curHp = 0f;
        }
    }
    
    public void SetDamagedDirectionVector(Vector3 vector)
    {
        _damagedDirectionVector = vector;
    }
    
    public Vector3 GetDamagedDirectionVector()
    {
        return _damagedDirectionVector;
    }
    
    public float GetJumpMaxHeight()
    {
        return _jumpMaxHeight;
    }
    
    public float GetJumpUpMaxTimer()
    {
        return _jumpUpMaxTimer;
    }

    public float GetJumpDownMaxTimer()
    {
        return _jumpDownMaxTimer;
    }

    public float GetAttackedAirborneUpTime()
    {
        return _attackedAirborneUpTime;
    }
    
    public float GetAttackedMaxHeight()
    {
        return _attackedMaxHeight;
    }
    
    public float GetAirboneUpMaxTimer()
    {
        return _airboneUpMaxTimer;
    }

    public float GetAirboneDownMaxTimer()
    {
        return _airboneDownMaxTimer;
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

    public void ChangeState(eRoleState state, eStateType stateType = eStateType.NONE)
    {
        Debug.Log($"[{name}][testState]State Change prev({_curRoleState}) cur({state}) count({_changeStates.Count})");
        _changeStates.Add(new StateInfo()
        {
            state = state,
            stateType = stateType
        });
    }
    
    public void ChangeRoleState(eRoleState roleState, eStateType stateType = eStateType.NONE)
    {
        ChangeState(roleState, stateType);
    }

    public void ResetMoveSpeed()
    {
        _moveSpeed = 0.0f;
        SetVelocity(Vector3.zero);
    }

    public void SetVelocity(Vector3 argVelocity)
    {
        _rigidbody.velocity = argVelocity;
        curVelocity = _rigidbody.velocity;
    }

    public void SetMoveSpeedToWalk()
    {
        _moveSpeed = _walkSpeedRatio * _agility;
    }

    public void SetMoveSpeedToRun()
    {
        _moveSpeed = _runSpeedRatio * _agility;
    }

    public float GetMoveSpeed()
    {
        return _moveSpeed;
    }

    public bool IsGround()
    {
        return _isGround;
    }
    
    public bool IsDead()
    {
        return _curHp <= 0f;
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

    // 엄todo : 캐릭터의 루팅 범위 기능 개발이 필요함(현재 캐릭터 크기의 캡슐로 처리)
    public Collider[] FindEnableAcquireItems()
    {
        Vector3 start = transform.position;
        start.y -= 0.7f;
        Vector3 end = transform.position;
        end.y += 0.7f;
        var result = Physics.OverlapCapsule(start, end, 0.2f);
        return result;
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

    public float GetTraceTargetDistanceXZ()
    {
        Vector3 myPosXZ = transform.position;
        myPosXZ.y = 0f;
        Vector3 targetPosXZ = traceTarget.transform.position;
        targetPosXZ.y = 0f;
        return Vector3.Distance(myPosXZ, targetPosXZ);
    }

    public bool IsInRange()
    {
        return (Vector3.Distance(traceTarget.transform.position, transform.position) > findRange);
    }

    public void OnHit(Collider other)
    {
        // if (eState.DEAD == _curState)
        //     return;
        if (eRoleState.DEAD == _curRoleState)
            return;
        // 피해 받았을때 진입
        // other : attacker
        _onHitQueue.Add(other);
    }

    public void RotateToPosition(Vector3 argPosition)
    {
        Vector3 vector = argPosition - transform.position;
        vector.y = 0;
        SetDirectionByVector3(vector);
    }

    public void OnAirborneLanding()
    {
        if (_curRoleState != eRoleState.DAMAGED_LANDING)
            ChangeState(eRoleState.DAMAGED_LANDING, eStateType.DAMAGE_LANDING);
    }

    public virtual void DeadDisable()
    {
        // 이건 deadState 이후 시체가 사라질 때 처리하는 함수로 쓰자.
        gameObject.SetActive(false);
    }

    public bool RefreshGroundCheckObjects()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(GetGroundBoxCenter(), _groundCollider.Size / 2, Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        _groundObjs = hits;
        return _groundObjs.Length > 0;
    }
    
    // public bool RefreshGroundCheckObjects()
    // {
    //     return _groundDetection.BottomSphereCast();
    // }
    
    private RaycastHit[] GetWallCheckObjects(Vector3 direction)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        
        if (false == TryGetComponent<CapsuleCollider>(out var collider))
            return null;
        var from = Vector3.forward;
        var to = direction;
        float rotateAngle = Vector3.SignedAngle(from, to, transform.up);
        var characterCenterPos = transform.position + collider.center;
        var wallPos = direction * (collider.radius + _wallCollider.Size.z / 2);
        var boxCenter = Matrix4x4.TRS(characterCenterPos, Quaternion.Euler(0, rotateAngle, 0), Vector3.one).GetPosition();
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter-wallPos, _wallCollider.Size / 2, direction, Quaternion.identity, 0f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
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
        
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, _wallCollider.Size / 2, Vector3.forward, Quaternion.identity, 0f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
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

    public void UpdateGroundHeight(bool forceUpdate = false)
    {
        float groundHeight = float.MinValue;
        var rayObjs = _groundObjs;
        if (null == rayObjs)
            return;
        
        Vector3 changePos = transform.position;
        foreach (var rayObj in rayObjs)
        {
            if (rayObj.collider.name.Contains("Slop"))
                return;
            if (rayObj.transform.TryGetComponent<Ground>(out var ground))
            {
                var changeHeightPosY = ground.heightPosY - transform.position.y;
                if (groundHeight < ground.heightPosY && (changeHeightPosY < 0.2f || forceUpdate))
                {
                    groundHeight = ground.heightPosY;
                    changePos.y = groundHeight;
                }
            }
        }
        
        transform.position = changePos;
    }

    private Vector3 GetGroundBoxHalfSize()
    {
        return _groundCollider.Size / 2;
    }
    
    private Vector3 GetGroundBoxCenter()
    {
        Vector3 boxCenter = transform.position;
        boxCenter.y -= _groundCollider.Size.y / 2;
        return boxCenter;
    }
    
    private Vector3 GetWallBoxCenter(Vector3 normVector)
    {
        // 엄todo : CapsuleCollider를 stand type에 collider 키고 끄도록 추가하기 _hitColliderMap에 관리하기
        if (false == TryGetComponent<CapsuleCollider>(out var collider))
        {
            Debug.LogError($"[testum]캡슐 콜리더 없음!");
            return Vector3.zero;
        }
        Vector3 boxCenter = transform.position + collider.center;   // 캐릭터 캡슐의 중앙
        boxCenter -= normVector * (collider.radius + (_wallCollider.Size.z / 2));    // 캐릭터 중앙에서 Wall Box 중앙 구하기
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
        foreach (var rangeType in _attackCollisionRangeMap.Keys)
        {
            _attackCollisionRangeMap[rangeType].EnableCollider(enable);
        }
    }
    public void ActiveAttackCollider(bool enable, HitboxType colliderType, AttackInfoData attackInfoData)
    {
        if (false == _attackCollisionRangeMap.ContainsKey(colliderType))
            return;
        _attackCollisionRangeMap[colliderType].EnableCollider(enable);
        _attackCollisionRangeMap[colliderType].SetAttackInfo(attackInfoData);
    }
    
    public void ActiveHitCollider(bool enable, HitColliderType colliderType)
    {
        if (false == _hitColliderMap.ContainsKey(colliderType))
            return;
        var list = _hitColliderMap[colliderType];
        foreach (var hitCollider in list)
        {
            hitCollider.gameObject.SetActive(enable);
        }

        // 엄todo : Collider들의 On/Off 관리용, HitCollider 관리용 분리하여 만들기
        if (TryGetComponent<CapsuleCollider>(out var capsuleCollider))
        {
            capsuleCollider.enabled = colliderType == HitColliderType.STAND;
        }
    }
    
    private Vector3 GetLeftRightWallBoxSize()
    {
        return new Vector3(_wallCollider.Size.z, _wallCollider.Size.y, _wallCollider.Size.x) / 2;
    }

    public void EquipDropItem(DropItem dropItem)
    {
        var item = dropItem.GetItem();
        if (false == item is ItemWeapon)
        {
            Debug.Log($"[testum]Equip fail! it's not ItemWeapon");
        }
        else
        {
            var itemWeapon = item as ItemWeapon;
            var colliderType = itemWeapon.GetEquipColliderType(); // 
            if (false == _equipPartColliderMap.TryGetValue(colliderType, out var partCollider))
            {
                Debug.LogError($"Character doesn't have colliderType({colliderType})");
                return;
            }
            var equipItem = Instantiate(itemWeapon, partCollider.transform);
            // equipItem.GetRole();
            _curRole = eRole.RAPIER;
            SettingRoleState("Rapier");

            Destroy(dropItem.gameObject);
        }
    }
    
    protected bool RegisterRoleState(eRoleState argRoleState, ActionKey argState, Type type)
    {
        if (argRoleState == eRoleState.NONE || argState == ActionKey.NONE)
            return false;
        if (false == _roleStateMap.ContainsKey(argRoleState))
        {
            State state = Activator.CreateInstance(type, this, argState) as State;
            _roleStateMap.Add(argRoleState, state);
        }
        else
        {
            Debug.Log($"[testRoleState]prev({_roleStateMap[argRoleState].GetType()})cur({type})typeIsSame({_roleStateMap[argRoleState].GetType() == type})");
            if (_roleStateMap[argRoleState].GetType() != type)
                _roleStateMap[argRoleState] = Activator.CreateInstance(type, this, argState) as State;
            else
                _roleStateMap[argRoleState].SetState(argState);
        }

        return true;
    }

    public CharacterStatData GetStatData()
    {
        var result = CharacterStatTable.GetData(_statKey);
        if (null == result)
            result = CharacterStatTable.GetData("Default");
        return result;
    }
    
    public AnimationFadeInfoData GetAnimFadeInfoData()
    {
        var result = AnimationFadeInfoTable.GetData(animFadeInfoKey);
        if (null == result)
            result = AnimationFadeInfoTable.GetData("Default");
        return result;
    }

    public float GetGravityDownTime()
    {
        return _gravityDownTime;
    }
    
    public float GetGravityDownHeight()
    {
        return _gravityDownHeight;
    }

    public float GetWalkTraceDistance()
    {
        return _walkTraceDistance;
    }
    
    public float GetAttackStartDistance()
    {
        return _attackStartDistance;
    }

    public Projectile SpawnAttackCube(ActionKey curState)
    {
        var projectileCube = Resources.Load<Projectile>("Prefabs/Projectile/ProjectileCube");
        projectileCube = Instantiate(projectileCube, _projectilePos.transform);
        projectileCube.transform.parent = null;

        projectileCube.Init(_directionVector.normalized, 3f, 3f, this);
        return projectileCube;
    }
}