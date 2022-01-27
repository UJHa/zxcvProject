using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        StartUI();

        SetWalkSpeed(0.005f);
        SetRunSpeed(0.015f);

        _rotationMap.Add(Direction.FRONT, new Vector3(0, 0, 0));
        _rotationMap.Add(Direction.BACK, new Vector3(0, 180, 0));
        _rotationMap.Add(Direction.LEFT, new Vector3(0, 90, 0));
        _rotationMap.Add(Direction.RIGHT, new Vector3(0, -90, 0));
        _rotationMap.Add(Direction.LEFT_FRONT, new Vector3(0, 45, 0));
        _rotationMap.Add(Direction.RIGHT_FRONT, new Vector3(0, -45, 0));
        _rotationMap.Add(Direction.LEFT_BACK, new Vector3(0, 135, 0));
        _rotationMap.Add(Direction.RIGHT_BACK, new Vector3(0, -135, 0));

        float diagonal = Mathf.Sqrt(2f) / 2f;

        _moveMap.Add(Direction.FRONT, new Vector3(0, 0, 1));
        _moveMap.Add(Direction.BACK, new Vector3(0, 0, -1));
        _moveMap.Add(Direction.LEFT, new Vector3(1, 0, 0));
        _moveMap.Add(Direction.RIGHT, new Vector3(-1, 0, 0));
        _moveMap.Add(Direction.LEFT_FRONT, new Vector3(diagonal, 0, diagonal));
        _moveMap.Add(Direction.RIGHT_FRONT, new Vector3(-diagonal, 0, diagonal));
        _moveMap.Add(Direction.LEFT_BACK, new Vector3(diagonal, 0, -diagonal));
        _moveMap.Add(Direction.RIGHT_BACK, new Vector3(-diagonal, 0, -diagonal));

        _inputMap.Add(Direction.FRONT, new KeyCode[] { KeyCode.DownArrow });
        _inputMap.Add(Direction.BACK, new KeyCode[] { KeyCode.UpArrow });
        _inputMap.Add(Direction.LEFT, new KeyCode[] { KeyCode.LeftArrow });
        _inputMap.Add(Direction.RIGHT, new KeyCode[] { KeyCode.RightArrow });
        _inputMap.Add(Direction.LEFT_FRONT, new KeyCode[] { KeyCode.DownArrow, KeyCode.LeftArrow });
        _inputMap.Add(Direction.RIGHT_FRONT, new KeyCode[] { KeyCode.DownArrow, KeyCode.RightArrow });
        _inputMap.Add(Direction.LEFT_BACK, new KeyCode[] { KeyCode.UpArrow, KeyCode.LeftArrow });
        _inputMap.Add(Direction.RIGHT_BACK, new KeyCode[] { KeyCode.UpArrow, KeyCode.RightArrow });

        _directionList.Add(Direction.LEFT_FRONT);
        _directionList.Add(Direction.RIGHT_FRONT);
        _directionList.Add(Direction.LEFT_BACK);
        _directionList.Add(Direction.RIGHT_BACK);
        _directionList.Add(Direction.FRONT);
        _directionList.Add(Direction.BACK);
        _directionList.Add(Direction.LEFT);
        _directionList.Add(Direction.RIGHT);

        //transform.Rotate(0, 90, 0);
        _direction = Direction.FRONT;

        stateMap.Add(eState.IDLE, new IdleState(this));
        stateMap.Add(eState.WALK, new WalkState(this));
        stateMap.Add(eState.RUN, new RunState(this));
        stateMap.Add(eState.JUMP, new JumpState(this));
        stateMap.Add(eState.ATTACK, new AttackState(this));
        stateMap.Add(eState.DEAD, new DeadState(this));

        _curState = eState.IDLE;

        stateMap[_curState].StartState();
    }

    protected override void StartUI()
    {
        base.StartUI();
        
        Vector3 sliderPos = transform.position;
        sliderPos.y += 2f;
        GameManager.Instance.SetPlayerUIPos(sliderPos);
        slider.transform.position = Camera.main.WorldToScreenPoint(sliderPos);
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        Vector3 sliderPos = transform.position;
        sliderPos.y += 2f;
        GameManager.Instance.SetPlayerUIPos(transform.position);
    }

    public override void DeadDisable()
    {
        base.DeadDisable();
        GameManager.Instance.OpenFinishDialog("실패");
    }
}
