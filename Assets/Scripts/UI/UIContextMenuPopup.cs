using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UIContextMenuPopup : MonoBehaviour
    {
        [SerializeField] private UIButton _menuButton;
        private List<UIButton> _menuBtnList = new();
        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<UIButton>(out var menuBtn))
                    _menuBtnList.Add(menuBtn);
            }

            DestroyMenus();
        }

        private void DestroyMenus()
        {
            foreach (UIButton btn in _menuBtnList)
            {
                GameObject.Destroy(btn.gameObject);
            }
            _menuBtnList.Clear();
        }

        public void Init()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        private void OnShow()
        {
            if (TryGetComponent<RectTransform>(out var rfm))
            {
                Debug.Log($"[testum]mouse({Input.mousePosition}) thisPosition({transform.position}) rfm({rfm.position}) rfm.anchoredPosition({rfm.anchoredPosition})");
                rfm.anchoredPosition = Input.mousePosition;
            }
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetupMenus(List<MenuAction> menuList)
        {
            DestroyMenus();
            foreach (var menu in menuList)
            {
                var btnObj = Instantiate(_menuButton, transform);
                btnObj.Init();
                btnObj.SetText(menu.btnName);
                btnObj.onClick.AddListener(() =>
                {
                    menu.btnAction?.Invoke();
                });
                _menuBtnList.Add(btnObj);
            }
        }
        
        public float GetPositionToSliderRate(UISlider slider, float posX)
        {
            Vector3[] worldCorners = new Vector3[4];
            
            slider.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            float width = worldCorners[2].x - worldCorners[0].x;
            float pinWidth = posX - worldCorners[0].x;
            return pinWidth / width;
        }
    }
}