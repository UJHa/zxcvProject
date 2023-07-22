using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UIAnimPlayerPage : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        // 엄todo UI와 연결 의존도 낮추기 위해 제거되어야 한다.
        private MoveSetCharacter _moveSetCharacter;
        
        public void Init(MoveSetCharacter moveSetCharacter, float minValue, float maxValue)
        {
            _moveSetCharacter = moveSetCharacter;
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.value = minValue;
        }
        
        private void Update()
        {
            UpdateSlider();
        }

        private void UpdateSlider()
        {
            if (null == _slider)
                return;
            if (_moveSetCharacter.IsPlayFinish())
            {
                if (false == _moveSetCharacter.IsAnimRateFinish())
                {
                    _slider.value = _slider.maxValue;
                    _moveSetCharacter.PlayFinish();
                }
                else
                    _moveSetCharacter.UpdateStateTime(_slider.value);
                // Debug.Log($"[testum]log1 _curNormValue({_curState.NormalizedTime}) curTimeValue({_curState.Time}) totalTime({_curState.Length})");
            }
            else
            {
                if (false == _moveSetCharacter.IsAnimRateFinish() && _moveSetCharacter.IsPlaying())
                    _slider.value = _moveSetCharacter.GetAnimRate();
                else
                    _moveSetCharacter.UpdateStateTime(_slider.value);

                // Debug.Log($"[testum]log2 _curNormValue({_curState.NormalizedTime}) curTimeValue({_curState.Time}) totalTime({_curState.Length}) test({_curState.Length - _curState.Time})");
            }
        }

        public void SliderOnPointerDown()
        {
            Debug.Log($"[testum]SliderOnPointerDown");
            UmUtil.SetSliderHold(true);
        }
    
        public void SliderOnPointerUp()
        {
            Debug.Log($"[testum]SliderOnPointerUp");
            UmUtil.SetSliderHold(false);
        }
    }
}