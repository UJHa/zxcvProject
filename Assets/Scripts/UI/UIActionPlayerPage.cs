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

public enum ActionParam
{
    START_RATE,
    END_RATE,
    ANIM_SPEED
}

namespace UI
{
    public class UIActionPlayerPage : MonoBehaviour
    {
        [Header("UI State")]
        [SerializeField] private AnimEditState _animEditState;
        [Header("Left View(UI)")]
        [SerializeField] private Transform _actionInfosParent;
        [SerializeField] private UITextInputField _actionInfoPrefab;
        [SerializeField] private UIButton _saveActionBtn;
        [Header("Right View(UI)")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private UIButton _actionInfoBtnPrefab;
        [Header("Bottom View(UI)")]
        [SerializeField] private UISlider _slider;
        [SerializeField] private Image _playRange;
        [SerializeField] private TextMeshProUGUI _curStateTxt;
        [SerializeField] private Button _play;
        [Header("UI Pin Rate")]
        [SerializeField] private float _startRate = 0f;
        [SerializeField] private float _endRate = 1f;

        private Image _pinStart = null;
        private Image _pinEnd = null;
        private Dictionary<string, ActionData> _actions = new();
        private Dictionary<ActionParam, UITextInputField> _actionInfoTextDict = new(); // _actionInfoPrefab 인스턴스 저장
        private Dictionary<string, UIButton> _actionInfoBtnDict = new(); // _actionInfoBtnPrefab 인스턴스 저장
        private string _selectActionName = "";
        
        private MoveSetCharacter _moveSetCharacter;
        
        public void Init(MoveSetCharacter moveSetCharacter)
        {
            _moveSetCharacter = moveSetCharacter;
            InitActionInspector();
            InitActionBtnList();

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
            
            _saveActionBtn.onClick.AddListener(() =>
            {
                //_selectActionName 선택된 액션 파라미터 json에 저장 기능 구현하기
                var editStartRate = _actionInfoTextDict[ActionParam.START_RATE].GetFieldData();
                var editEndRate = _actionInfoTextDict[ActionParam.END_RATE].GetFieldData();
                var editAnimSpeed = _actionInfoTextDict[ActionParam.ANIM_SPEED].GetFieldData();
                Debug.Log($"[saveAction]editStartRate({editStartRate})editEndRate({editEndRate})editAnimSpeed({editAnimSpeed})");
                SaveJsonFile();
            });
            
            _play.onClick.AddListener(() =>
            {
                PlayAnimUI();
            });
        }

        private void SaveJsonFile()
        {
            ActionTable.SetActionData(_selectActionName, new(_actions[_selectActionName]));
            ActionTable.Export();
        }

        private void InitActionInspector()
        {
            // anim_speed
            {
                var infoField = Instantiate(_actionInfoPrefab, _actionInfosParent);
                infoField.SetGuideText("AnimSpeed : ");
                infoField.SetPlaceHolder("min/max:0/~");
                infoField.SetInputField("");
                _actionInfoTextDict.Add(ActionParam.ANIM_SPEED, infoField);
            }
            // startRate
            {
                var infoField = Instantiate(_actionInfoPrefab, _actionInfosParent);
                infoField.SetGuideText("StartRate : ");
                infoField.SetPlaceHolder("min/max:0/1");
                infoField.SetInputField("");
                _actionInfoTextDict.Add(ActionParam.START_RATE, infoField);
            }
            // endRate
            {
                var infoField = Instantiate(_actionInfoPrefab, _actionInfosParent);
                infoField.SetGuideText("EndRate : ");
                infoField.SetPlaceHolder("min/max:0/1");
                infoField.SetInputField("");
                _actionInfoTextDict.Add(ActionParam.END_RATE, infoField);
            }
        }

        private void InitActionBtnList()
        {
            var allActions = ActionTable.GetActionList();
            foreach (var actionData in allActions)
            {
                if (actionData.actionName == eState.NONE.ToString())
                    continue;
                _actions.Add(actionData.actionName, actionData);
                Debug.Log($"[testum]actionData({actionData})");
                _actionInfoBtnPrefab.name = actionData.actionName;
                if (_actionInfoBtnDict.ContainsKey(actionData.actionName))
                {
                    Debug.LogError($"ScrollView have actionName's UIButton({actionData.actionName})");
                    continue;
                }
                var btnObj = Instantiate(_actionInfoBtnPrefab, _scrollRect.content);
                btnObj.name = _actionInfoBtnPrefab.name;
                btnObj.Init();
                btnObj.SetText(actionData.actionName);
                _actionInfoBtnDict.Add(actionData.actionName, btnObj);
            }

            foreach (var actionData in _actions.Values)
            {
                var btnObj = _actionInfoBtnDict[actionData.actionName];
                btnObj.onClick.AddListener(() =>
                {
                    Debug.Log($"[testum]Click button name({btnObj.name})");
                    _selectActionName = actionData.actionName;
                    SelectButton();
                });
            }

            _selectActionName = _actions[eState.FIGHTER_IDLE.ToString()].actionName; 
            _actionInfoBtnDict[_selectActionName].onClick.Invoke();
        }

        private void SelectButton()
        {
            // 엄todo : 툴이라서 일단 모든 버튼 이름 갱신 처리
            // 이전 선택된 버튼 이름 기존으로 롤백("[Select]"글자 지우기)
            // 나중에 이름이 아닌 다른 방식으로 액션 수정 여부 표기 시 구조 변경 필요
            foreach (var actionName in _actionInfoBtnDict.Keys)
            {
                var uiButton = _actionInfoBtnDict[actionName];
                if (actionName.Equals(_selectActionName))
                {
                    uiButton.SetText($"[Select]{uiButton.name.Split("(")[0]}");
                }
                else
                {
                    uiButton.SetText(uiButton.name.Split("(")[0]);
                }

                _actions[actionName] = ActionTable.GetActionData(actionName);
            }

            var actionData = _actions[_selectActionName];
            _moveSetCharacter.ChangeAction(actionData);
            _actionInfoTextDict[ActionParam.START_RATE].SetInputField(actionData.startTimeRatio.ToString());
            _actionInfoTextDict[ActionParam.END_RATE].SetInputField(actionData.endTimeRatio.ToString());
            _actionInfoTextDict[ActionParam.ANIM_SPEED].SetInputField(actionData.speed.ToString());
        }

        private void VisibleActionRange()
        {
            var actionData = _actions[_selectActionName];
            var editStartRate = _actionInfoTextDict[ActionParam.START_RATE].GetFieldData();
            var editEndRate = _actionInfoTextDict[ActionParam.END_RATE].GetFieldData();
            var editAnimSpeed = _actionInfoTextDict[ActionParam.ANIM_SPEED].GetFieldData();
            var startRatio = UmUtil.StringToFloat(editStartRate);
            var endRatio = UmUtil.StringToFloat(editEndRate);
            var speed = UmUtil.StringToFloat(editAnimSpeed);
            actionData.startTimeRatio = Mathf.Floor(startRatio * 100f) /  100f;
            actionData.endTimeRatio = Mathf.Floor(endRatio * 100f) /  100f;
            actionData.speed = Mathf.Floor(speed * 100f) /  100f;
            _playRange.rectTransform.offsetMin = new(_slider.GetComponent<RectTransform>().sizeDelta.x * actionData.startTimeRatio, _playRange.rectTransform.offsetMin.y);
            _playRange.rectTransform.offsetMax = new(-_slider.GetComponent<RectTransform>().sizeDelta.x * (1f - actionData.endTimeRatio), _playRange.rectTransform.offsetMax.y);
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
                _moveSetCharacter.PlayAnim(_actions[_selectActionName]);
                SetAnimEditState(AnimEditState.Play);
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayAnimUI();
            }
            UpdateSlider();
            UpdateText();
            VisibleActionRange();
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