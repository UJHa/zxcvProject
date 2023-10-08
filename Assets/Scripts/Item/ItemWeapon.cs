using UnityEngine;

namespace Item
{
    public class ItemWeapon : ItemBase
    {
        [SerializeField] private Transform _equipSocket;
        [SerializeField] private ColliderType _colliderType;

        public Vector3 GetEquipSocket()
        {
            return Vector3.zero;
        }
        
        public ColliderType GetEquipColliderType()
        {
            return _colliderType;
        }
    }
}