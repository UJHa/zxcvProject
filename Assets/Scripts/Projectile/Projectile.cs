using System;
using DataClass;
using DG.Tweening;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private float _moveDistance;
    [SerializeField] private Character _owner;
    private float _curMoveTotal = 0f;
    
    private Tween _moveTween = null;
    
    public void Init(Vector3 argDirection, float argSpeed, float argDistance, Character owner)
    {
        _curMoveTotal = 0f;
        _moveSpeed = argSpeed;
        _moveDirection = argDirection;
        _moveDistance = argDistance;
        _owner = owner;
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = _moveDirection * _moveSpeed * Time.fixedDeltaTime;
        _curMoveTotal += moveVector.magnitude;
        transform.position += moveVector;
        if (_curMoveTotal > _moveDistance)
            Destroy(gameObject);
    }

    public void SetAttackInfo(AttackInfoData attackInfoData)
    {
        if (TryGetComponent<AttackCollider>(out var attackCollider))
        {
            attackCollider.SetOwner(_owner);
            attackCollider.SetAttackInfo(attackInfoData);
        }
    }
}