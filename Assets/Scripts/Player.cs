using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    void Start()
    {
        StartUI();

        _directionVector = Vector3.back;
        
        _moveSet.Init(this);
        _moveSet.RegisterAction(eState.ATTACK, KeyCode.C, eState.IDLE, new ActionInfo("Animation/Lucy_FightFist01_1", 0f, 0.7f, ActorHitColliderType.LEFT_HAND, 0.15f, 0.4f));
        _moveSet.RegisterAction(eState.ATTACK2, KeyCode.C, eState.ATTACK, new ActionInfo("Animation/Lucy_FightFist01_2", 0.1f, 0.5f, ActorHitColliderType.RIGHT_HAND, 0.0f, 0.3f));
        _moveSet.RegisterAction(eState.ATTACK3, KeyCode.C, eState.ATTACK2, new ActionInfo("Animation/Lucy_FightFist02_2b_1", 0f, 0.4f, ActorHitColliderType.LEFT_HAND, 0.1f, 0.2f));
        _moveSet.RegisterAction(eState.DAMAGED, KeyCode.X, eState.IDLE, new ActionInfo("Animation/Damaged", 0f, 1.0f, ActorHitColliderType.NONE, 0.0f, 1.0f));
        _moveSet.RegisterAction(eState.DAMAGED, KeyCode.X, eState.DAMAGED, new ActionInfo("Animation/Damaged", 0f, 1.0f, ActorHitColliderType.NONE, 0.0f, 1.0f));

        // 이거를 게임 도중에 할 수도 있음.. 겟앰프드의 야수 캐릭터 같은 경우? or 캐릭터 체력 상태별 다른 공격 모션을 주고 싶을 때
        _stateMap.Add(eState.IDLE, new IdleState(this, eState.IDLE));
        _stateMap.Add(eState.WALK, new WalkState(this, eState.WALK));
        _stateMap.Add(eState.RUN, new RunState(this, eState.RUN));
        _stateMap.Add(eState.RUNSTOP, new RunStopState(this, eState.RUNSTOP));
        _stateMap.Add(eState.JUMP_UP, new JumpUpState(this, eState.JUMP_UP));
        _stateMap.Add(eState.JUMP_DOWN, new JumpDownState(this, eState.JUMP_DOWN));
        _stateMap.Add(eState.LANDING, new LandingState(this, eState.LANDING));
        _stateMap.Add(eState.ATTACK, new PunchOneState(this, eState.ATTACK));
        _stateMap.Add(eState.ATTACK2, new PunchTwoState(this, eState.ATTACK2));
        _stateMap.Add(eState.ATTACK3, new PunchThreeState(this, eState.ATTACK3));
        _stateMap.Add(eState.DAMAGED, new DamagedState(this, eState.DAMAGED));
        _stateMap.Add(eState.DEAD, new DeadState(this, eState.DEAD));

        _curState = eState.IDLE;

        _stateMap[_curState].StartState();
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HitCollider>(out var hitCollider))
        {
            Debug.Log($"[testum][name:{name}]be hit other({other.name})");
            var attacker = hitCollider.GetOwner();
            if (attacker != this)
            {
                ChangeState(eState.DAMAGED);
            }
        }
    }
}
