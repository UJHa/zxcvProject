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

        public void SetupMenus(List<string> menuList)
        {
            DestroyMenus();
            foreach (var contextName in menuList)
            {
                var btnObj = Instantiate(_menuButton, transform);
                btnObj.Init();
                btnObj.SetText(contextName);
                btnObj.onClick.AddListener(() =>
                {
                    
                });
                _menuBtnList.Add(btnObj);
            }
        }
    }
}