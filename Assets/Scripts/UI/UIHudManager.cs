using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHudManager : MonoBehaviour
    {
        private Dictionary<int, Slider> _sliders = new();
        private Dictionary<int, Character> _characters = new();
        public void Init()
        {
            
        }

        public void AddPlayerSlider(int hashCode, Character character)
        {
            if (_sliders.ContainsKey(hashCode))
            {
                ReleaseLog.LogError("[UIHudManager]캐릭터 HashCode가 Slider 객체 참조중이라 생성 불가");
                return;
            }
            try
            {
                GameObject prefab = Resources.Load("Prefabs/HpSlider") as GameObject;
                GameObject hpSlider = GameObject.Instantiate(prefab, transform, true);
                hpSlider.transform.SetAsFirstSibling();
                var slider = hpSlider.GetComponent<Slider>();
                slider.gameObject.SetActive(true);
                
                _sliders.Add(hashCode, slider);
                _characters.Add(hashCode, character);
            }
            catch (Exception e)
            {
                ReleaseLog.LogError("[UIHudManager]Slider 생성 실패");
            }
        }

        public void SetSliderValue(int hashCode, float rateValue)
        {
            if (false == _sliders.ContainsKey(hashCode))
            {
                ReleaseLog.LogError("[UIHudManager]캐릭터의 Slider 객체가 존재하지 않음");
                return;
            }

            var slider = _sliders[hashCode];
            slider.value = rateValue;
        }

        private void LateUpdate()
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            foreach (var hashCode in _characters.Keys)
            {
                var character = _characters[hashCode];
                var slider = _sliders[hashCode];
                Vector3 sliderPos = character.transform.position;
                sliderPos.y += 2f;
                _sliders[hashCode].transform.position = Camera.main.WorldToScreenPoint(sliderPos);
                
                slider.gameObject.transform.localScale = Vector3.one;
            }
        }
    }
}