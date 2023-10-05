using UnityEngine;

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
        if (null == _ownCharacter)
            Debug.LogError("[testum]GetOwner is null!");
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
}