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
        private Dictionary<ActionParam, UITextInputField> _actionInfoTextDict = new(); // _actionInfoPrefab 인스턴스 저장
        private Dictionary<string, UIButton> _actionInfoBtnDict = new(); // _actionInfoBtnPrefab 인스턴스 저장
        private string _selectActionName = "";

        // 엄todo UI와 연결 의존도 낮추기 위해 제거되어야 한다.
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
            });
            
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

        private void InitActionInspector()
        {
            // startRate
            {
                var a = Instantiate(_actionInfoPrefab, _actionInfosParent);
                a.SetGuideText("StartRate : ");
                a.SetPlaceHolder("min/max:0/1");
                a.SetInputField("");
                _actionInfoTextDict.Add(ActionParam.START_RATE, a);
            }
            // endRate
            {
                var a = Instantiate(_actionInfoPrefab, _actionInfosParent);
                a.SetGuideText("EndRate : ");
                a.SetPlaceHolder("min/max:0/1");
                a.SetInputField("");
                _actionInfoTextDict.Add(ActionParam.END_RATE, a);
            }
            // anim_speed
            {
                var a = Instantiate(_actionInfoPrefab, _actionInfosParent);
                a.SetGuideText("AnimSpeed : ");
                a.SetPlaceHolder("min/max:0/~");
                a.SetInputField("");
                _actionInfoTextDict.Add(ActionParam.ANIM_SPEED, a);
            }
        }

        private void InitActionBtnList()
        {
            var allActions = ActionTable.GetActionList();
            foreach (var actionData in allActions)
            {
                if (actionData.actionName == eState.NONE.ToString())
                    continue;
                _actions.Add(actionData);
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

            foreach (var actionData in _actions)
            {
                var btnObj = _actionInfoBtnDict[actionData.actionName];
                btnObj.onClick.AddListener(() =>
                {
                    Debug.Log($"[testum]Click button name({btnObj.name})");
                    _selectActionName = actionData.actionName;
                    SelectButton(actionData);
                });
            }

            _selectActionName = _actions[0].actionName; 
            _actionInfoBtnDict[_selectActionName].onClick.Invoke();
        }

        private void SelectButton(ActionData actionData)
        {
            // 이전 선택된 버튼 이름 기존으로 롤백("[Select]"글자 지우기)
            // 엄todo : 툴이라서 일단 모든 버튼 이름 갱신 처리
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
            }
            
            _moveSetCharacter.ChangeAction(actionData);
            _actionInfoTextDict[ActionParam.START_RATE].SetInputField(actionData.startTimeRatio.ToString());
            _actionInfoTextDict[ActionParam.END_RATE].SetInputField(actionData.endTimeRatio.ToString());
            _actionInfoTextDict[ActionParam.ANIM_SPEED].SetInputField(1.ToString());
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