using DataClass;
using Unity.VisualScripting;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
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