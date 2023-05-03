using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 엄todo : 여기에 여러개의 Action을 넣어서 공격 처리 구현하기 
public class ComboState : State
{
    private int maxComboCount = 3;
    private int curComboCount = 1;

    private List<Action> _actions = new();
    public ComboState(Character character, eState eState) : base(character, eState)
    {
        // _actions.Add( new Action("Punch1", _animator));
        // _actions.Add(new Action("Punch2", _animator));
        // _actions.Add(new Action("Punch3", _animator));
    }

    public override void StartState()
    {
        curComboCount = 1;
        _animator.Play("Punch1");
        // _actions[curComboCount - 1].PlayStart();
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        // if (IsComboAction())
        if (true)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                _character.ChangeState(eState.IDLE);
            }
            else if (Input.GetKeyDown(KeyCode.C) && _character.IsGround() && curComboCount < maxComboCount)
            {
                _animator.Rebind();
                curComboCount++;
                _animator.Play("Punch1");
                // _actions[curComboCount - 1].PlayStart();
            }
        }
    }

    private bool IsComboAction()
    {
        bool result = false;
        // foreach (var actionName in _actions)
        // {
        //     result |= _animator.GetCurrentAnimatorStateInfo(0).IsName(actionName);
        // }

        return result;
    }
}