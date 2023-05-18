using UnityEngine;

public class HitCollider : MonoBehaviour
{
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
}