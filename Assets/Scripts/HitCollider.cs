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
        if (_hitColliderType == HitColliderType.AIRBORNE)
            Debug.Log($"[testum]trigger! mine({name}) other({other})");
        if (null != _character)
        {
            if (other.TryGetComponent<AttackCollider>(out var attackCollider))
                _character.OnHit(other);
            else if (other.TryGetComponent<Ground>(out var ground) && _hitColliderType == HitColliderType.AIRBORNE)
                _character.OnAirborneLanding(ground);
        }
    }

    public HitColliderType GetHitType()
    {
        return _hitColliderType;
    }
}