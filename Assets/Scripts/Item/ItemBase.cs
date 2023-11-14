using UnityEngine;

public enum eItemType
{
    NONE,
    WEAPON,
    POTION,
}

public abstract class ItemBase : MonoBehaviour
{
    public abstract eItemType GetItemType();
}