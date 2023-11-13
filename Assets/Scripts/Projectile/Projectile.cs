using System;
using System.Collections.Generic;
using DataClass;
using DG.Tweening;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private float _moveDistance;
    [SerializeField] private Character _owner;
    // 플러그인 로직 이식 코드(개선 필요)
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject hit;
    [SerializeField] private GameObject flash;
    [SerializeField] private float hitOffset = 0f;
    [SerializeField] private bool UseFirePointRotation;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);
    [SerializeField] private GameObject[] Detached;
    
    private float _curMoveTotal = 0f;
    private HitInfo _hitInfo = new();
    private HashSet<int> _hitInstanceIds = new();

    public void Init(Vector3 argDirection, float argSpeed, float argDistance, Character owner)
    {
        _curMoveTotal = 0f;
        _moveSpeed = argSpeed;
        _moveDirection = argDirection;
        _moveDistance = argDistance;
        _owner = owner;
        if (null == _rigidbody)
            _rigidbody = GetComponent<Rigidbody>();
        _hitInstanceIds.Clear();
    }

    private void FixedUpdate()
    {
        // Vector3 moveVector = _moveDirection * _moveSpeed * Time.fixedDeltaTime;
        // _curMoveTotal += moveVector.magnitude;
        // // transform.position += moveVector;
        // if (_curMoveTotal > _moveDistance)
        //     Destroy(gameObject);
        if (_moveSpeed != 0)
        {
            _rigidbody.velocity = transform.forward * _moveSpeed;
            //transform.position += transform.forward * (speed * Time.deltaTime);         
        }
    }
    
    // 엄todo : 탄알 충돌 정밀 처리는 FixedUpdate에서 Physics를 통해서 하는 방식 고려하기
    void OnCollisionEnter(Collision collision)
    {
        ProcessHit(collision.gameObject);
        //Lock all axes movement and rotation
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _moveSpeed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        //Spawn hit effect on collision
        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

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

    private void ProcessHit(GameObject gameObj)
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
}