using UnityEngine;

namespace UI
{
    public class UIWidget : MonoBehaviour
    {
        private GameObject _ownWidget;
        public void SetOwner(GameObject obj)
        {
            _ownWidget = obj;
        }
        
        public GameObject GetOwner()
        {
            return _ownWidget;
        }
    }
}