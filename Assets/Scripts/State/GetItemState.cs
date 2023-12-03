using System;
using Debug = UnityEngine.Debug;

public class GetItemState : State
{
    private bool canPickUp = false;
    private DropItem _dropItem = null;
    public GetItemState(Character character, ActionKey actionKey) : base(character, actionKey)
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
            ReleaseLog.LogInfo($"[{_character.name}][testLoot]colliderName({collider.name}) isDropItem({collider.TryGetComponent<DropItem>(out var teno)})");
            if (collider.TryGetComponent<DropItem>(out var dropItem))
            {
                canPickUp = true;
                _dropItem = dropItem;
                break;
            }
        }

        ExecuteGetItem();
    }

    private void ExecuteGetItem()
    {
        if (null == _dropItem)
        {
            _character.ChangeRoleState(eRoleState.IDLE);
            return;
        }

        var itemType = _dropItem.GetItem().GetItemType();
        switch (itemType)
        {
            case eItemType.NONE:
                _character.ChangeRoleState(eRoleState.IDLE);
                break;
            case eItemType.WEAPON:
                _character.ResetMoveSpeed();
                _moveSet.Play(_action);
                _character.EquipDropItem(_dropItem);
                break;
            case eItemType.POTION:
                _character.RecoveryHealth(20f);
                _character.ChangeRoleState(eRoleState.IDLE);
                break;
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
            _character.ChangeRoleState(eRoleState.IDLE);
            return;
        }
        if (_character.IsGround())
        {
            if (_moveSet.IsAnimationFinish())
            {
                _character.ChangeRoleState(eRoleState.IDLE);
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