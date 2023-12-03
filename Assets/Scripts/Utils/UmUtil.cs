using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class UmUtil
    {
        private static int testFrame = 120;
        private static float oneFrameTime = 0f;
        
        private static bool _isSliderHold = false;

        public static string GetResourceJsonPath()
        {
            return $"{Application.dataPath}/Resources/Json";
        }
        
        public static string GetDataClassPath()
        {
            return $"{Application.dataPath}/Scripts/DataClass";
        }

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

        public static float GetWidth(RectTransform rectTransform)
        {
            return rectTransform.rect.width;
        }

        public static float StringToFloat(string num)
        {
            if (string.IsNullOrEmpty(num))
                return 0f;
            if (float.TryParse(num, out var result))
                return result;
            else
            {
                ReleaseLog.LogError($"StringToFloat Failed! num({num})");
                return 0f;
            }
        }
        
        public static List<Transform> GetAllChildList(Transform originTfm)
        {
            List<Transform> allChilds = new();
            Stack<Transform> stackTfms = new();

            stackTfms.Push(originTfm);
            while (stackTfms.Count > 0)
            {
                var curChild = stackTfms.Pop();
                allChilds.Add(curChild);

                foreach (Transform child in curChild)
                {
                    stackTfms.Push(child);
                }
            }

            return allChilds;
        }

        public static string ConvertJsonToTableClassName(string fileName)
        {
            var jsonName = fileName.Split('.')[0];
                
            string fileClassName = $"{jsonName}Table";
            return $"{char.ToUpper(fileClassName[0])}{fileClassName.Substring(1)}";
        }
    }
}