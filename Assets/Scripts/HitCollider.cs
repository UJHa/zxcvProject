using System;
using UnityEngine;

public enum HitColliderType
{
    STAND,
    AIRBORNE,
}

public class HitCollider : MonoBehaviour
{
    [SerializeField] private HitColliderType _hitColliderType;
    [SerializeField] private Character _character;

    private void OnTriggerEnter(Collider other)
    {
        if (null != _character)
            _character.OnHit(other);
    }

    public HitColliderType GetHitType()
    {
        return _hitColliderType;
    }
}