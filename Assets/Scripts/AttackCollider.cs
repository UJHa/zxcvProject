using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private AttackPartColliderType _colliderType = AttackPartColliderType.NONE;
    private Character _ownCharacter = null;
    private AttackType _attackType;

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

    public void SetAttackType(AttackType attackType)
    {
        _attackType = attackType;
    }

    public AttackType GetAttackType()
    {
        return _attackType;
    }
}