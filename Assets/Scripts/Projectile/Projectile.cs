using System;
using System.Collections.Generic;
using DataClass;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private float _maxMoveDistance;
    [SerializeField] private Character _owner;
    [SerializeField] private float _curMoveDistance;
    // 플러그인 로직 이식 코드(개선 필요)
    [SerializeField] private Rigidbody _rigidbody;
    // 엄todo : Resources.load 미리 해두고 탄 하나당 Instansiate 하나씩 들고 있도록 작업하기
    [SerializeField] private GameObject hit;
    [SerializeField] private GameObject flash;
    [SerializeField] private GameObject[] Detached;
    [SerializeField] private SphereCollider _sphereCollider;
    
    [SerializeField] private float hitOffset = 0f;
    [SerializeField] private bool UseFirePointRotation;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);
    private Vector3 _prevPos = Vector3.zero;
    
    private Vector3 _startPosition;
    private bool _callDestroy = false;
    private bool _isHit = false;
    
    private float _curMoveTotal = 0f;
    private HitInfo _hitInfo = new();
    private HashSet<int> _hitInstanceIds = new();

    public void Init(Vector3 argDirection, float argSpeed, float argMaxDistance, Character owner)
    {
        _curMoveTotal = 0f;
        _moveSpeed = argSpeed;
        _moveDirection = argDirection;
        _maxMoveDistance = argMaxDistance;
        _owner = owner;
        _isHit = false;
        _callDestroy = false;
        _startPosition = _prevPos = transform.position;

        if (null == _rigidbody)
            _rigidbody = GetComponent<Rigidbody>();
        if (null == _sphereCollider)
            _sphereCollider = GetComponent<SphereCollider>();
        _hitInstanceIds.Clear();
    }

    private void FixedUpdate()
    {
        if (_moveSpeed != 0)
        {
            _rigidbody.velocity = transform.forward * _moveSpeed;
        }

        if (!_isHit)
            _isHit = TargetCast();
    }

    private void LateUpdate()
    {
        if (_callDestroy)
            return;
        _curMoveDistance = (transform.position - _startPosition).magnitude;
        if (_curMoveDistance >= _maxMoveDistance)
        {
            Destroy(gameObject);
            _callDestroy = true;
        }
    }
    
    void ProcessHit(RaycastHit argHit)
    {
        SendHitInfo(argHit.collider.gameObject);
        //Lock all axes movement and rotation
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _moveSpeed = 0;
        
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, argHit.normal);
        Vector3 pos = argHit.point + argHit.normal * hitOffset;

        //Spawn hit effect on collision
        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(argHit.point + argHit.normal); }

            // 엄todo : Destory 호출은 즉시 호출로 변경하기
            // 엄todo : duration은 update에서 관리하도록 수정하기
            //Destroy hit effects depending on particle Duration time
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

        // 엄todo : Destory 호출은 즉시 호출로 변경하기
        // 엄todo : duration은 update에서 관리하도록 수정하기
        //Removing trail from the projectile on cillision enter or smooth removing. Detached elements must have "AutoDestroying script"
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
                Destroy(detachedPrefab, 1);
            }
        }
        //Destroy projectile on collision
        Destroy(gameObject);
    }

    private void SendHitInfo(GameObject gameObj)
    {
        if (_hitInstanceIds.Contains(gameObj.GetInstanceID()))
            return;
        if (gameObj.TryGetComponent<Character>(out var character))
        {
            _hitInstanceIds.Add(gameObj.GetInstanceID());
            character.OnHit(_hitInfo);
        }
        else if (gameObj.TryGetComponent<HitCollider>(out var hitCollider))
        {
            _hitInstanceIds.Add(gameObj.GetInstanceID());
            hitCollider.OnHit(_hitInfo);
        }
    }

    public void SetHitInfo(AttackInfoData attackInfoData)
    {
        _hitInfo.Attacker = _owner;
        _hitInfo.AttackInfoData = attackInfoData;
        _hitInfo.RaycastHit = default;
    }

    public HitInfo GetHitInfo()
    {
        return _hitInfo;
    }
    
    public bool TargetCast()
    {
        LayerMask layer = ~0; // everything
        var moveDistance = (transform.position - _prevPos).magnitude;
        if (Physics.SphereCast(_prevPos, _sphereCollider.radius, transform.forward, out RaycastHit hit, moveDistance, layer))
        {
            _hitInfo.RaycastHit = hit;
            ProcessHit(hit);
            return true;
        }

        _prevPos = transform.position;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector3.zero, _sphereCollider.radius);
        Gizmos.matrix = Matrix4x4.identity;
    }
}