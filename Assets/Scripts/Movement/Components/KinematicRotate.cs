using ECM.Common;
using UnityEngine;

namespace ECM.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicRotate : MonoBehaviour
    {
        #region FIELDS

        [SerializeField]
        private float _rotationSpeed = 30.0f;
        [SerializeField]
        private Vector3 _rotateRatio = new(0f, 1f, 0f);

        #endregion

        #region PRIVATE FIELDS

        private Rigidbody _rigidbody;

        private float _angle;

        #endregion

        #region PROPERTIES

        public float rotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = Mathf.Clamp(value, -360.0f, 360.0f); }
        }

        public float angle
        {
            get { return _angle; }
            set { _angle = ECM.Common.Utils.WrapAngle(value); }
        }

        #endregion

        #region MONOBEHAVIOUR

        public void OnValidate()
        {
            rotationSpeed = _rotationSpeed;
        }

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        public void FixedUpdate()
        {
            angle += rotationSpeed * Time.deltaTime;
            
            var rotation = Quaternion.Euler(_rotateRatio.normalized * angle);
            _rigidbody.MoveRotation(rotation);
        }

        #endregion
    }
}
