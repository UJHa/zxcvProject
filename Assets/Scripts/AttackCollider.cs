using Unity.VisualScripting;
using UnityEngine;

public enum AttackRangeType
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
    [SerializeField] private AttackRangeType _colliderType = AttackRangeType.NONE;
    private Character _ownCharacter = null;
    private HitboxInfo _hitboxInfo;

    public void SetOwner(Character character)
    {
        _ownCharacter = character;
    }
    
    public Character GetOwner()
    {
        return _ownCharacter;
    }

    public void SetAttackInfo(HitboxInfo hitboxInfo)
    {
        _hitboxInfo = hitboxInfo;
    }

    public HitboxInfo GetAttackInfo()
    {
        return _hitboxInfo;
    }

    public void EnableCollider(bool enable)
    {
        if (TryGetComponent<Collider>(out var collider))
            collider.enabled = enable;
    }
}