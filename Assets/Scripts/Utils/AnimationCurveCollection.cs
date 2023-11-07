using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utils
{
    public class AnimationCurveCollection : MonoBehaviour
    {
        [SerializeField] public AnimationCurve jumpUp;
        [SerializeField] public AnimationCurve jumpDown;
        [SerializeField] public AnimationCurve airBoneUp;
        [SerializeField] public AnimationCurve airBoneDown;
        [SerializeField] public AnimationCurve knockBack;
        [SerializeField] public AnimationCurve flyAwayGround;
        [SerializeField] public AnimationCurve flyAwayHeight;

        public AnimationCurve GetAnimCurve(string key)
        {
            FieldInfo fieldInfo = this.GetType().GetField(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            // Debug.Log($"[testVar]GetAnimCurve({fieldInfo.GetValue(this)})");
            AnimationCurve result = fieldInfo.GetValue(this) as AnimationCurve;
            return result;
        }
        
        public AnimationCurve CreateAnimCurve(string key)
        {
            FieldInfo fieldInfo = this.GetType().GetField(key);
            Debug.Log($"[testVar]CreateAnimCurve({fieldInfo.GetValue(this)})");
            AnimationCurve result = fieldInfo.GetValue(this) as AnimationCurve;
            result = new AnimationCurve(result.keys);
            return result;
        }
        
        public AnimationCurve CreateAnimCurve(string key, float xMaxValue)
        {
            FieldInfo fieldInfo = this.GetType().GetField(key);
            Debug.Log($"[testVar]GetAnimCurve({fieldInfo.GetValue(this)})");
            AnimationCurve result = fieldInfo.GetValue(this) as AnimationCurve;
            result = new AnimationCurve(result.keys);
            MakeFixedDeltaTimeCurve(result, xMaxValue);
            return result;
        }
        
        private void MakeFixedDeltaTimeCurve(AnimationCurve curve, float xMaxValue)
        {
            // 초단위로 커브 만들고 maxTime 통해서 커브 시간 변경
            List<float> temp = new();
            var divideLength = Time.fixedDeltaTime;
            var count = (int)(1f / divideLength);
            for (int i = 0; i < count; i++)
            {
                temp.Add(curve.Evaluate(divideLength * i));
            }
            temp.Add(1f);
        
            while (curve.keys.Length > 0)
            {
                curve.RemoveKey(0);
            }

            for (int i = 0; i < temp.Count; i++)
            {
                curve.AddKey(i * divideLength * xMaxValue, temp[i]);
            }
        }
    }
}