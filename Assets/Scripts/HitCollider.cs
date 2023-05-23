using UnityEngine;

public class HitCollider : MonoBehaviour
{
    [SerializeField] private ActorHitColliderType _colliderType = ActorHitColliderType.NONE;
    private Character _ownCharacter = null;

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

    public bool IsSame(ActorHitColliderType colliderType)
    {
        return colliderType == _colliderType;
    }
}