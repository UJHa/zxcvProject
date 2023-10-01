using System;
using System.Collections.Generic;
using DataClass;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [Header("UI object")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private UIButton _infoBtnPrefab;
        [SerializeField] private UISlider _slider;
        [SerializeField] private TextMeshProUGUI _curStateTxt;
        [SerializeField] private Button _play;
        [SerializeField] private Button _playPin;
        [SerializeField] private Button _exportBtn;
        [Header("UI Pin Rate")]
        [SerializeField] private float _startRate = 0f;
        [SerializeField] private float _endRate = 1f;

        private Image _pinStart = null;
        private Image _pinEnd = null;
        private List<ActionData> _actions = new();
        private Dictionary<string, UIButton> _scrollBtnDict = new(); // action name을 key로 설정

        // 엄todo UI와 연결 의존도 낮추기 위해 제거되어야 한다.
        private MoveSetCharacter _moveSetCharacter;
        
        public void Init(MoveSetCharacter moveSetCharacter)
        {
            _moveSetCharacter = moveSetCharacter;
            var prefabName = _infoBtnPrefab.name;
            var allActions = ActionTable.GetActionList();
            foreach (var actionData in allActions)
            {
                if (actionData.actionName == eState.NONE.ToString())
                    continue;
                _actions.Add(actionData);
                Debug.Log($"[testum]actionData({actionData})");
                _infoBtnPrefab.name = actionData.actionName;
                if (_scrollBtnDict.ContainsKey(actionData.actionName))
                {
                    Debug.LogError($"ScrollView have actionName's UIButton({actionData.actionName})");
                    continue;
                }
                var btnObj = Instantiate(_infoBtnPrefab, _scrollRect.content);
                btnObj.Init();
                btnObj.SetText(actionData.actionName);
                _scrollBtnDict.Add(actionData.actionName, btnObj);
            }

            foreach (var actionData in _actions)
            {
                var btnObj = _scrollBtnDict[actionData.actionName];
                btnObj.onClick.AddListener(() =>
                {
                    Debug.Log($"[testum]Click button name({btnObj.name})");
                    SelectButton(btnObj, $"[Select]{btnObj.name.Split("(")[0]}", actionData);
                });
            }

            _infoBtnPrefab.name = prefabName;

            _slider.Init(moveSetCharacter.GetStartAnimRate(), moveSetCharacter.GetEndAnimRate());

            if (_slider.TryGetComponent<EventTrigger>(out var trigger))
            {
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(UmUtil.SliderOnPointerDown);
                    trigger.triggers.Add(entry);
                }
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerUp;
                    entry.callback.AddListener(UmUtil.SliderOnPointerUp);
                    trigger.triggers.Add(entry);
                }
                {
                    EventTrigger.Entry entry = new();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener(UmUtil.SliderOnPointerClick);
                    trigger.triggers.Add(entry);
                }
            }
            
            _play.onClick.AddListener(() =>
            {
                PlayAnimUI();
            });
            
            _playPin.onClick.AddListener(() =>
            {
                PlayPinAnim();
            });
            
            _exportBtn.onClick.AddListener(() =>
            {
                ExportAction();
            });
        }

        private void SelectButton(UIButton btnObj, string argName, ActionData actionData)
        {
            // 이전 선택된 버튼 이름 기존으로 롤백("[Select]"글자 지우기)
            // 엄todo : 툴이라서 일단 모든 버튼 이름 갱신 처리
            // 나중에 이름이 아닌 다른 방식으로 액션 수정 여부 표기 시 구조 변경 필요
            foreach (var uiButton in _scrollBtnDict.Values)
            {
                uiButton.SetText(uiButton.name.Split("(")[0]);
            }
            
            btnObj.SetText(argName);
            _moveSetCharacter.ChangeAction(actionData);
        }

        private void PlayAnimUI()
        {
            if (_moveSetCharacter.IsPlaying())
            {
                SetAnimEditState(AnimEditState.Stop);
                _moveSetCharacter.PauseAnim();
            }
            else
            {
                // _moveSetCharacter.SetActionStartRate(0f);
                // _moveSetCharacter.SetActionEndRate(1f);
                _moveSetCharacter.PlayAnim();
                SetAnimEditState(AnimEditState.Play);
            }
        }
        
        private void PlayPinAnim()
        {
            if (_moveSetCharacter.IsPlaying())
            {
                SetAnimEditState(AnimEditState.Stop);
                _moveSetCharacter.PauseAnim();
            }
            else
            {
                _moveSetCharacter.PlayPinAnim();
                SetAnimEditState(AnimEditState.Play);
            }
        }
        
        private void ExportAction()
        {
            _moveSetCharacter.ExportCurAction();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayAnimUI();
            }
            UpdateSlider();
            UpdateText();
        }

        private void UpdateSlider()
        {
            if (null == _slider)
                return;
            if (UmUtil.IsSliderHold())
            {
                SetAnimEditState(AnimEditState.Stop);
                _moveSetCharacter.PauseAnim();
                _moveSetCharacter.UpdateStateTime(_slider.GetValue());
            }
            else if (_moveSetCharacter.IsPlaying())
            {
                _slider.SetValue(_moveSetCharacter.GetAnimRate());
                if (_moveSetCharacter.IsAnimRateFinish())
                {
                    SetAnimEditState(AnimEditState.Stop);
                    _slider.SetValue(_moveSetCharacter.GetEndAnimRate());
                    _moveSetCharacter.PlayFinish();
                }
            }
        }
        
        private void UpdateText()
        {
            if (null == _curStateTxt)
                return;
            _curStateTxt.text = _animEditState.ToString();
        }

        public void SetAnimEditState(AnimEditState editState)
        {
            _animEditState = editState;
        }

        private void OnDestroy()
        {
            _play.onClick.RemoveAllListeners();
        }
    }
}