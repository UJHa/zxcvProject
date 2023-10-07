using System.Diagnostics;
using Animancer;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class GetItemState : State
{
    public GetItemState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _character.ResetMoveSpeed();
        _action.Play();
    }

    public override void FixedUpdateState()
    {
        // var groundObjs = _character.GetGroundCheckObjects();
    }

    public override void EndState()
    {
        
    }

    public override void UpdateState()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (_character.IsGround())
        {
            if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
        }
    }
    
    private void UpdateMoveInput()
    {
        
    }
}