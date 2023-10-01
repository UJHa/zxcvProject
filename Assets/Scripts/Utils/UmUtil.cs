using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class UmUtil
    {
        private static int testFrame = 120;
        private static float oneFrameTime = 0f;
        
        private static bool _isSliderHold = false;

        public static float GetOnFrameTime()
        {
            if (0f == oneFrameTime)
                oneFrameTime = 1f / testFrame;
            return oneFrameTime;
        }

        public static void SetSliderHold(bool isHold)
        {
            _isSliderHold = isHold;
        }
        
        public static bool IsSliderHold()
        {
            return _isSliderHold;
        }
        
        public static void SliderOnPointerDown(BaseEventData argData)
        {
            Debug.Log($"[testum]SliderOnPointerDown");
            UmUtil.SetSliderHold(true);
        }
    
        public static void SliderOnPointerUp(BaseEventData argData)
        {
            Debug.Log($"[testum]SliderOnPointerUp");
            UmUtil.SetSliderHold(false);
        }
        
        public static void SliderOnPointerClick(BaseEventData argData)
        {
            Debug.Log($"[testum]argData({argData.selectedObject})");
            Debug.Log($"[testum]SliderOnPointerClick");
            UmUtil.SetSliderHold(false);
        }

        public static T StringToEnum<T>(string name) where T : struct
        {
            bool success = Enum.TryParse<T>(name, out var result);
            if (success)
                return result;
            return default(T);
        }
    }
}