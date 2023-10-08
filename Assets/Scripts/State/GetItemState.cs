using System.Diagnostics;
using Animancer;
using Item;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class GetItemState : State
{
    private bool canPickUp = false;
    private DropItem _dropItem = null;
    public GetItemState(Character character, eState eState) : base(character, eState)
    {
    }

    public override void StartState()
    {
        base.StartState();
        canPickUp = false;
        _dropItem = null;
        
        var colliders = _character.FindEnableAcquireItems();
        foreach (var collider in colliders)
        {
            Debug.Log($"[{_character.name}][testLoot]colliderName({collider.name}) isDropItem({collider.TryGetComponent<DropItem>(out var teno)})");
            if (collider.TryGetComponent<DropItem>(out var dropItem))
            {
                canPickUp = true;
                _dropItem = dropItem;
                break;
            }
        }
        
        if (canPickUp)
        {
            _character.ResetMoveSpeed();
            _action.Play();

            _character.EquipDropItem(_dropItem);
        }
        else
        {
            _character.ChangeState(eState.IDLE);
        }
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
        if (false == canPickUp)
        {
            _character.ChangeState(eState.IDLE);
            return;
        }
        if (_character.IsGround())
        {
            if (_action.IsAnimationFinish())
            {
                _character.ChangeState(eState.IDLE);
            }
        }
        UpdateInput();
    }

    private void UpdateInput()
    {
    }
    
    private void UpdateMoveInput()
    {
        
    }
}