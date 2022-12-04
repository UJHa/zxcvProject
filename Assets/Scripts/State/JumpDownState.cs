using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpDownState : State
{
    private float _jumpTimer = 0f;
    public JumpDownState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpdown start");
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        Vector3 boxCenter = character.transform.position;
        boxCenter.y -= character._downBoxHeight / 2;
        Vector3 boxHalfSize = new Vector3(1f, character._downBoxHeight, 1f) / 2;  // 캐스트할 박스 크기의 절반 크기. 이렇게 하면 가로 2 세로 2 높이 2의 크기의 공간을 캐스트한다.
        int layerMask = 1;
        layerMask = layerMask << LayerMask.NameToLayer("Ground");
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, boxHalfSize, Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        
        // if (hits.Length > 1)
        // {
        //     Debug.Log("=====");
        //     foreach (var hi in hits)
        //     {
        //         if (character._downBoxHeight > Vector3.Distance(boxCenter, hi.transform.position))
        //             Debug.Log($"name : {hi.collider.name} distance : {Vector3.Distance(boxCenter, hi.transform.position)}");
        //     }
        // }
        if (hits.Length > 0)
        {
            Debug.Log("[testumLanding]isGround!");
            character.ChangeState(eState.IDLE);
            return;
        }
        
        _jumpTimer += Time.fixedDeltaTime;
        character.GetRigidbody().velocity = new Vector3(0f, character.GetJumpDownVelocity(_jumpTimer), 0f) ; 
        Debug.Log($"[jumpdown]timer({_jumpTimer}) GetVelocity({character.GetJumpDownVelocity(_jumpTimer)}), position({character.transform.position}), rigid pos({character.GetRigidbody().position})");
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpdown end");
    }
}