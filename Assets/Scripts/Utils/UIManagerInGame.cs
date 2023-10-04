using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class UIManagerInGame : UIManager
    {
        private static UIManagerInGame instance = null;
        public static UIManagerInGame Instance => instance;
        // DevScene용 UI 페이지
        public UIHudManager hudManager;
        
        protected override void InitInstance()
        {
            if (instance)
            {
                Destroy(this.gameObject);
            }

            instance = this;
        }

        private void Update()
        {
            
        }
    }
}