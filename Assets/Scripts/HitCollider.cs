using System;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    [SerializeField] private Character _character;

    private void OnTriggerEnter(Collider other)
    {
        if (null != _character)
            _character.OnHit(other);
    }
}