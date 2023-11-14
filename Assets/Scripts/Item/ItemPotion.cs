using UnityEngine;

public class ItemPotion : ItemBase
{
    public override eItemType GetItemType()
    {
        return eItemType.POTION;
    }
}