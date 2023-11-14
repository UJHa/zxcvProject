using UnityEngine;

public class ItemHelmet : ItemBase
{
    [SerializeField] private Renderer _meshRenderer;

    public Renderer GetMeshRenderer()
    {
        return _meshRenderer;
    }


    public override eItemType GetItemType()
    {
        return eItemType.NONE;
    }
}