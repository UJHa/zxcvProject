using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public enum AnimEditState
{
    Play,
    Stop
}

namespace UI
{
    public class UIAnimPlayerPage : MonoBehaviour
    {
        [Header("UI State")]
        [SerializeField] private AnimEditState _animEditState;
        
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _curStateTxt;
        [SerializeField] private Button _play;

        // 엄todo UI와 연결 의존도 낮추기 위해 제거되어야 한다.
        private MoveSetCharacter _moveSetCharacter;
        
        public void Init(MoveSetCharacter moveSetCharacter, float minValue, float maxValue)
        {
            _moveSetCharacter = moveSetCharacter;
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.value = minValue;
            
            _play.onClick.AddListener(() =>
            {
                _moveSetCharacter.PlayAnim();
                _animEditState = AnimEditState.Play;
            });
        }
        
        private void Update()
        {
            UpdateSlider();
            UpdateText();
        }

        private void UpdateSlider()
        {
            if (null == _slider)
                return;
            if (_moveSetCharacter.IsPlayFinish())
            {
                _animEditState = AnimEditState.Stop;
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
        
        private void UpdateText()
        {
            if (null == _curStateTxt)
                return;
            _curStateTxt.text = _animEditState.ToString();
        }

        private void OnDestroy()
        {
            _play.onClick.RemoveAllListeners();
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