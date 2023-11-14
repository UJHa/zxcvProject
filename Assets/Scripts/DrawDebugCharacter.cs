using UnityEngine;

public class DrawDebugCharacter
{
    private Character _character;
    private ColliderCube _groundCollider;
    public void Init(Character character)
    {
        _character = character;
    }

    public void SetGroundCollider(ColliderCube colliderCube)
    {
        _groundCollider = colliderCube;
    }
    
    public void DrawUpdate()
    {
        DrawGroundCheckBox();
        DrawHitBox();
    }

    private void DrawHitBox()
    {
        
    }

    private void DrawGroundCheckBox()
    {
        Vector3 center = new(0f, - _groundCollider.Size.y / 2, 0f);
        _groundCollider.gizmoPos = center;
        Gizmos.matrix = Matrix4x4.TRS(_character.transform.position, Quaternion.identity, Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, _groundCollider.Size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}