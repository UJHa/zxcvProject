using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ItemBase _item;
    
    public ItemBase GetItem()
    {
        return _item;
    }
}