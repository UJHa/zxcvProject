using UnityEngine;

public class PartCollider : MonoBehaviour
{
    [SerializeField] private ColliderType _colliderType = ColliderType.NONE;
    private Character _ownCharacter = null;

    public void SetOwner(Character character)
    {
        _ownCharacter = character;
    }
}