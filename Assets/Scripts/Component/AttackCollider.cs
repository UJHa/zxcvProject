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
    SWORD_A,
    SWORD_B,
}

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private HitboxType _colliderType = HitboxType.NONE;
    private Character _ownCharacter = null;
    private AttackInfoData _attackInfoData;

    public void SetOwner(Character character)
    {
        _ownCharacter = character;
    }
    
    public Character GetOwner()
    {
        return _ownCharacter;
    }

    public void SetAttackInfo(AttackInfoData attackInfoData)
    {
        _attackInfoData = attackInfoData;
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
}