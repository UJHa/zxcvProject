using UnityEngine;

public class DrawDebugCharacter
{
    private Character _character;
    private ColliderCube _groundCollider;
    private ColliderCube _wallCollider;
    public void Init(Character character)
    {
        _character = character;
    }

    public void SetGroundCollider(ColliderCube colliderCube)
    {
        _groundCollider = colliderCube;
    }

    public void SetWallCollider(ColliderCube colliderCube)
    {
        _wallCollider = colliderCube;
    }
    
    public void DrawUpdate()
    {
        DrawGroundCheckBox();
        DrawWallCheckBox();
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

    private void DrawWallCheckBox()
    {
        if (_character.TryGetComponent<CapsuleCollider>(out var collider))
        {
            for (int i = 0; i < 8; i++)
            {
                var rotateAngle = i * 45;
                var characterCenterPos = _character.transform.position + collider.center; 
                var pivot = 0.5f;
                var pivotPos = _wallCollider.Size.z / 2;
                Gizmos.matrix = Matrix4x4.TRS(characterCenterPos, Quaternion.Euler(0, rotateAngle, 0), Vector3.one);
                Gizmos.color = Color.red;
                var cubePos = Vector3.forward * (collider.radius + _wallCollider.Size.z / 2);
                Gizmos.DrawWireCube(cubePos, _wallCollider.Size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}