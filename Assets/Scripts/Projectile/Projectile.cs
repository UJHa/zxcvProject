using System;
using DG.Tweening;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private float _moveDistance;
    private float _curMoveTotal = 0f;
    
    private Tween _moveTween = null;
    
    public void Init(Vector3 argDirection, float argSpeed, float argDistance)
    {
        _curMoveTotal = 0f;
        _moveSpeed = argSpeed;
        _moveDirection = argDirection;
        _moveDistance = argDistance;
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = _moveDirection * _moveSpeed * Time.fixedDeltaTime;
        _curMoveTotal += moveVector.magnitude;
        transform.position += moveVector;
        if (_curMoveTotal > _moveDistance)
            Destroy(gameObject);
    }

}