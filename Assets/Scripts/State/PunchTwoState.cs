using Animancer;
using UnityEngine;
using UnityEditor;

public class PunchTwoState : AttackState
{
    private AnimancerState _curState;

    public PunchTwoState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        _curState = _action.Play();
        _character.ActiveAttackColliders(true, ActorHitColliderType.LEFT_HAND);
    }

    public override void FixedUpdateState()
    {
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void UpdateState()
    {
        if (_character.IsGround())
        {
            var nextState = _moveSet.DetermineNextState(_character.GetCurState(), KeyCode.C);
            if (eState.NONE != nextState)
                _character.ChangeState(nextState, eStateType.INPUT);
            else if (_action.IsFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
        }
        // if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animName))
        // {
        //     if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        //     {
        //         _character.ChangeState(eState.ATTACK3);
        //     }
        // }
    }
}