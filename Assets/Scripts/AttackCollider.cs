using DataClass;
using Unity.VisualScripting;
using UnityEngine;

public enum HitboxType
{
    NONE,
    PUNCH_A,
    PUNCH_B,
    KICK_A,
    KICK_B,
    SWORD,
}

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private HitboxType _colliderType = HitboxType.NONE;
    private Character _ownCharacter = null;
    private AttackInfoData _attackInfoData;
    private string _hitKey;

    public void SetOwner(Character character)
    {
        _ownCharacter = character;
    }
    
    public Character GetOwner()
    {
        return _ownCharacter;
    }

    public void SetAttackInfo(string hitKey, AttackInfoData attackInfoData)
    {
        _attackInfoData = attackInfoData;
        _hitKey = hitKey;
    }

    public AttackInfoData GetAttackInfo()
    {
        return _attackInfoData;
    }

    public void EnableCollider(bool enable)
    {
        if (TryGetComponent<Collider>(out var collider))
            collider.enabled = enable;
    }

    public string GetHitKey()
    {
        return _hitKey;
    }
}