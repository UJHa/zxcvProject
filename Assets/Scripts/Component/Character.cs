using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Animancer;
using DataClass;
using DG.Tweening;
using ECM.Components;
using UnityEngine;
using UnityEngine.Serialization;
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
[Serializable]
public class ColliderCube
{
    public Vector3 Size;
    public Vector3 gizmoPos;
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

public enum HitFxType
{
    NONE,
    WHITE,
    RED,
    BLUE,
}

public class Character : MonoBehaviour
{
    [Header("[ Test rigidbody velocity]")]
    [SerializeField] private Vector3 _moveVelocity = Vector3.zero;
    [SerializeField] private Vector3 _platformVelocity = Vector3.zero;
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
    [Header("[ Max Airbone Height ]")]
    [SerializeField] private bool _maxAirborneHeight = false;
    [SerializeField] private bool _isGuard = false;

    [SerializeField] private float _gravityDownTime = 0.6f;
    [SerializeField] private float _gravityDownHeight = 2f;
    
    [Header("[ Ground Collider ]")]
    [SerializeField] private ColliderCube _groundCollider = new ColliderCube
    {
        Size = new(0.2f, 0.03f, 0.2f),
        gizmoPos = default
    };
    
    [Header("[ RootPrefab ]")]
    [SerializeField] private Transform _rootTransform;

    [Header("[ Hit Collider ]")]
    [SerializeField] private List<HitCollider> _hitColliders = new();
    private Dictionary<HitColliderType, List<HitCollider>> _hitColliderMap = new();
    
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
    
    [SerializeField] protected GameObject _projectilePos;

    protected DrawDebugCharacter _drawDebug;

    public bool _isGround = false;

    private List<StateInfo> _changeStates = new();

    private Tween _rotationTween = null;

    private RaycastHit[] _groundObjs = null;

    protected MoveSet _moveSet = new();
    protected AnimancerComponent _animancer;
    
    private List<HitInfo> _onHitQueue = new();

    protected float _attackedMaxHeight = 0f;
    protected float _attackedAirborneUpTime = 0f;

    private CharacterMovement _characterMovement;
    private GroundDetection _groundDetection;
    private Coroutine _lateFixedUpdateCoroutine = null;

    private Dictionary<HitFxType, GameObject> _hitFxObjects = new();
    
    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out var rigidbody))
            _rigidbody = rigidbody;
        
        _drawDebug = new();
        _drawDebug.Init(this);
        _drawDebug.SetGroundCollider(_groundCollider);

        if (TryGetComponent<AnimancerComponent>(out var animancer))
            _animancer = animancer;

        if (TryGetComponent<CharacterMovement>(out var characterMovement))
            _characterMovement = characterMovement;
        if (TryGetComponent<GroundDetection>(out var groundDetection))
            _groundDetection = groundDetection;

        SettingProjectilePos();

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
            partColliderData.Collider.SetOwner(this);
            if (false == _equipPartColliderMap.ContainsKey(partColliderData.Type))
                _equipPartColliderMap.Add(partColliderData.Type, partColliderData.Collider);
        }

        InitStats();

        InitHitFxs();

        InitStates();
        StartUI();
    }

    public void SetGrounding(bool enable)
    {
        if (!enable)
        {
            _characterMovement.DisableGrounding();
        }
        _isGround = IsGround();
    }
    
    public void OnEnable()
    {
        if (_lateFixedUpdateCoroutine != null)
            StopCoroutine(_lateFixedUpdateCoroutine);

        _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());
    }

    public void OnDisable()
    {
        // Stop LateFixedUpdate coroutine

        if (_lateFixedUpdateCoroutine != null)
            StopCoroutine(_lateFixedUpdateCoroutine);
    }

    private void SettingProjectilePos()
    {
        var obj = Resources.Load<GameObject>("Prefabs/Projectile/ProjectilePos");
        _projectilePos = Instantiate(obj, transform);
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
        ReleaseLog.LogInfo($"[testRoleTable]length({properties.Length})");
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
                ReleaseLog.LogInfo($"[setRoleState]{roleState}:[{actionKey}][{stateClassType}][{isSuccess}]");
            }
        }
    }

    protected void InitStats()
    {
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

    private void InitHitFxs()
    {
        _hitFxObjects.Add(HitFxType.WHITE, Resources.Load<GameObject>("Prefabs/StatusFx/Hits/CFXM_Hit_C White"));
        _hitFxObjects.Add(HitFxType.RED, Resources.Load<GameObject>("Prefabs/StatusFx/Hits/CFXM_Hit_A Red"));
        _hitFxObjects.Add(HitFxType.BLUE, Resources.Load<GameObject>("Prefabs/StatusFx/Hits/CFXM_Hit_B Blue"));
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
        DetectGround();
        
        _roleStateMap[_curRoleState].FixedUpdateState();
    }

    public void DetectGround()
    {
        float castDistance = _groundDetection.castDistance;
        if (!_characterMovement.isGrounded)
        {
            castDistance = _moveVelocity.y * Time.fixedDeltaTime;
        }
        _groundDetection.castDistance = Mathf.Abs(castDistance);
        _characterMovement.DetectGroundCustom();
        if (_characterMovement.isGrounded)
        {
            _moveVelocity.y = 0f;
            _rigidbody.velocity = _moveVelocity + _characterMovement.platformVelocity;
            _characterMovement.SnapToGround();
        }
        else
        {
            _rigidbody.velocity = _moveVelocity;
        }
    }
    
    private IEnumerator LateFixedUpdate()
    {
        var waitTime = new WaitForFixedUpdate();
            
        while (true)
        {
            yield return waitTime;

            // Solve any possible overlap after internal physics update

            var p = transform.position;
            var q = transform.rotation;

            // Attempt to snap to a moving platform (if any)

            if (_characterMovement.isOnGround && _characterMovement.isOnPlatform)
                _characterMovement.SnapToPlatform(ref p, ref q);
            
            // Update rigidbody

            _rigidbody.MovePosition(p);
            _rigidbody.MoveRotation(q);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_roleStateMap.ContainsKey(_curRoleState))
            return;
        _roleStateMap[_curRoleState].DrawGizmosUpdateState();
        _drawDebug?.DrawUpdate();
    }
    
    public void ClearAttackInfoData()
    {
        _attackedMaxHeight = 0f;
        _attackedAirborneUpTime = 0f;
    }

    void Update()
    {
        while (_onHitQueue.Count > 0)
        {
            var hitInfo = _onHitQueue[0];
            ProcessHit(hitInfo);
            _onHitQueue.RemoveAt(0);
        }

        if (_changeStates.Count > 0)
        {
            eRoleState state = eRoleState.NONE;
            if (_changeStates.Count == 1)
                state = _changeStates[0].state;
            else
                state = SelectNextState();
            _roleStateMap[_curRoleState].EndState();
            _prevRoleState = _curRoleState;
            _curRoleState = state;
            _roleStateMap[state].StartState();
            _changeStates.Clear();
        }

        _roleStateMap[_curRoleState].UpdateState();
    }
    
    private void ProcessHit(HitInfo hitInfo)
    {
        var attacker = hitInfo.Attacker;
        AttackInfoData attackInfo = hitInfo.AttackInfoData;
        AttackType attackType = UmUtil.StringToEnum<AttackType>(attackInfo.attackType);
        _attackedMaxHeight = attackInfo.airborneHeight;
        _attackedAirborneUpTime = attackInfo.airborneTime;
        var damage = attackInfo.damageRatio * attacker._strength;
        SetDamage(damage);
        var closePos = hitInfo.RaycastHit.point;
        switch (attackType)
        {
            case AttackType.NONE:
                break;
            case AttackType.NORMAL:
                if (false == IsGuard())
                {
                    if (IsGround())
                    {
                        if (IsDead())
                            ChangeRoleState(eRoleState.DEAD);
                        else
                            ChangeRoleState(eRoleState.NORMAL_DAMAGED);
                        InstantiateHitFx(HitFxType.WHITE, closePos);
                    }
                    else
                    {
                        ChangeRoleState(eRoleState.AIRBORNE_DAMAGED);
                        InstantiateHitFx(HitFxType.BLUE, closePos);
                    }
                }
                else
                {
                    ChangeRoleState(eRoleState.GUARD_DAMAGED);
                }
                break;
            case AttackType.AIRBORNE:
                // 방향을 때린 상대의 방향으로 회전시키기
                RotateToPosition(attacker.transform.position);
                ChangeRoleState(eRoleState.AIRBORNE_DAMAGED);
                InstantiateHitFx(HitFxType.RED, closePos);
                break;
            case AttackType.AIR_POWER_DOWN:
                ChangeRoleState(eRoleState.AIRBORNE_POWER_DOWN_DAMAGED);
                InstantiateHitFx(HitFxType.RED, closePos);
                break;
            case AttackType.KNOCK_BACK:
                RotateToPosition(attacker.transform.position);
                SetDamagedDirectionVector(attacker.GetDirectionVector());
                ChangeRoleState(eRoleState.KNOCK_BACK_DAMAGED);
                InstantiateHitFx(HitFxType.RED, closePos);
                break;
            case AttackType.FLY_AWAY:
                RotateToPosition(attacker.transform.position);
                SetDamagedDirectionVector(attacker.GetDirectionVector());
                ChangeRoleState(eRoleState.FLY_AWAY_DAMAGED);
                InstantiateHitFx(HitFxType.RED, closePos);
                break;
        }
    }

    private void InstantiateHitFx(HitFxType hitFxKey, Vector3 closePos)
    {
        var hitFxObj = Instantiate(_hitFxObjects[hitFxKey]);
        hitFxObj.transform.position = closePos;
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
        Vector3 moveDirection = direction;
        Vector3 moveVelocity = moveDirection * _moveSpeed;
        return moveVelocity;
    }

    private Vector3[] GetReverseVectors(Vector3 vector)
    {
        return new[] { vector, Quaternion.Euler(0, 45, 0) * vector, Quaternion.Euler(0, -45, 0) * vector };
    }

    public void SetDirectionByVector3(Vector3 argVector, float rotateTime = 0.1f)
    {
        _directionVector = argVector;
        // var euler = GetEuler(_directionVector);
        if (null != _rotationTween)
            _rotationTween.Kill();
        var rot = GetRotation(_directionVector);
        _rotationTween = transform.DORotateQuaternion(rot, rotateTime);
    }

    public void StopRotateDirection()
    {
        if (null != _rotationTween)
            _rotationTween.Kill();
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

    public virtual Vector3 GetMoveInputVector()
    {
        return Vector3.zero;
    }
    
    public void ChangeRoleState(eRoleState roleState, eStateType stateType = eStateType.NONE)
    {
        ReleaseLog.LogInfo($"[{name}][testState]Request Change prev({_curRoleState}) cur({roleState}) count({_changeStates.Count})pos({transform.position})");
        _changeStates.Add(new StateInfo()
        {
            state = roleState,
            stateType = stateType
        });
    }

    public void ResetMoveSpeed()
    {
        _moveSpeed = 0.0f;
        SetVelocity(Vector3.zero);
    }

    public void SetVelocity(Vector3 argVelocity)
    {
        _moveVelocity = argVelocity;
        _platformVelocity = _characterMovement.platformVelocity;
    }

    public Vector3 GetVelocity()
    {
        return _moveVelocity;
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

    public void OnHit(HitInfo hitInfo)
    {
        if (eRoleState.DEAD == _curRoleState)
            return;
        _onHitQueue.Add(hitInfo);
    }

    public void RotateToPosition(Vector3 argPosition)
    {
        Vector3 vector = (argPosition - transform.position).normalized;
        vector.y = 0;
        SetDirectionByVector3(vector);
    }

    public virtual void DeadDisable()
    {
        gameObject.SetActive(false);
    }

    public bool IsGround()
    {
        return _characterMovement.isGrounded;
    }

    public void SetPositionY(float groundHeight)
    {
        ReleaseLog.LogInfo($"groundHeight({groundHeight})");
        var transform1 = transform;
        Vector3 pos = transform1.position;
        transform1.position = new Vector3(pos.x, groundHeight, pos.z);
    }

    public void UpdateGroundHeight(bool forceUpdate = false)
    {
        var changePos = transform.position;
        changePos.y = _groundDetection.groundPoint.y;
        transform.position = changePos;
        ReleaseLog.LogInfo($"[testUpdate]position({changePos})");
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

    public void EquipDropItem(DropItem dropItem)
    {
        var item = dropItem.GetItem();
        if (false == item is ItemWeapon)
        {
            ReleaseLog.LogInfo($"[testum]Equip fail! it's not ItemWeapon");
        }
        else
        {
            var itemWeapon = item as ItemWeapon;
            var colliderType = itemWeapon.GetEquipColliderType(); // 
            if (false == _equipPartColliderMap.TryGetValue(colliderType, out var partCollider))
            {
                ReleaseLog.LogError($"Character doesn't have colliderType({colliderType})");
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
            ReleaseLog.LogInfo($"[testRoleState]prev({_roleStateMap[argRoleState].GetType()})cur({type})typeIsSame({_roleStateMap[argRoleState].GetType() == type})");
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
        var projectileCube = Resources.Load<Projectile>("Prefabs/Projectile/ProjectileSample");
        projectileCube = Instantiate(projectileCube, transform.TransformPoint(_projectilePos.transform.localPosition), transform.rotation);

        projectileCube.Init(projectileCube.transform.forward.normalized, 11f, 5f, this);
        return projectileCube;
    }

    public RaycastHit[] HitBoxCast(Vector3 offset, Vector3 size)
    {
        LayerMask layer = ~0; // everything
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + transform.rotation * offset, size / 2f, transform.forward, transform.rotation, 0f, layer);
        List<RaycastHit> result = new();
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<HitCollider>(out var hitCollider)
                && hitCollider.GetCharacter().GetInstanceID() != GetInstanceID())
            {
                Vector3 rayStartPos = transform.position;
                rayStartPos.y += offset.y;
                // 정확한 타격 지점 확인용 물리 처리
                var rayHits = Physics.RaycastAll(rayStartPos, transform.forward, size.z, layer);
                foreach (var rayHit in rayHits)
                {
                    if (rayHit.collider == hit.collider)
                    {
                        result.Add(rayHit);
                    }
                }
            }
        }
        return result.ToArray();
    }

    public void RecoveryHealth(float addHp)
    {
        _curHp += addHp;
        UIManagerInGame.Instance.hudManager.SetSliderValue(GetHashCode(), _curHp / _fullHp);
        if (_curHp >= _fullHp)
        {
            _curHp = _fullHp;
        }
    }
    public void SetMaxHeightAirborne(bool v)
    {
        _maxAirborneHeight = v;
    }

    public bool GetMaxHeightAirborne()
    {
        return _maxAirborneHeight;
    }

    public void SetGuard(bool isGuard)
    {
        _isGuard = isGuard;
    }

    public bool IsGuard()
    {
        return _isGuard;
    }
}