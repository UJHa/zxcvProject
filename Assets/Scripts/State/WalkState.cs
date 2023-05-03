using System;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class WalkState : State
{
    public WalkState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        string name = "";
        name = name.Equals("") && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ? "Idle" : name;
        name = name.Equals("") && _animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") ? "Walk" : name;
        name = name.Equals("") && _animator.GetCurrentAnimatorStateInfo(0).IsName("Run") ? "Run" : name;
        name = name.Equals("") && _animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") ? "Jump" : name;
        name = name.Equals("") && _animator.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd") ? "JumpEnd" : name;
        Debug.Log($"[walkstart] prev anim{name}");
        _character.SetMoveSpeedToWalk();

        _animator.CrossFade("Walk", _character.walkStart);
    }

    public override void FixedUpdateState()
    {
        var groundObjs = _character.GetGroundCheckObjects();
        if (0 == groundObjs.Length)
        {
            _character.ChangeState(eState.JUMP_DOWN);
        }
        else
            _character.UpdateGroundHeight();
    }

    public override void EndState()
    {
    }

    public override void UpdateState()
    {
        UpdateInput();
    }
    
    void UpdateInput()
    {
        var vector = InputManager.Instance.GetButtonAxisRaw();
        if (Vector3.zero == vector)
        {
            _character.ChangeState(eState.IDLE);
            return;
        }

        if (InputManager.Instance.GetButtonDown(KeyBindingType.JUMP))
        {
            _character.ChangeState(eState.JUMP_UP, eStateType.INPUT);
            return;
        }
        
        _character.SetDirectionByVector3(vector);
        _character.MovePosition(vector);
    }
}