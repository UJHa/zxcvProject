using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpUpState : State
{
    private float _jumpTimer = 0f;
    public JumpUpState(Character character) : base(character)
    {
    }

    public override void StartState()
    {
        _jumpTimer = 0f;
        Debug.Log($"[State] jumpup start");
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (_jumpTimer >= character.GetJumpUpMaxTimer())
        {
            Debug.Log($"[testlog] jump up update fin?");
            character.ChangeState(eState.JUMP_DOWN);
            return;
        }
        _jumpTimer += Time.fixedDeltaTime;
        character.GetRigidbody().velocity = new Vector3(0f, character.GetJumpUpVelocity(_jumpTimer), 0f) ; 
        Debug.Log($"[jumpup]timer({_jumpTimer}) GetVelocity({character.GetJumpUpVelocity(_jumpTimer)}), position({character.transform.position}), rigid pos({character.GetRigidbody().position})");
        // if (_checkGround)
        // {
        //     Vector3 boxCenter = transform.position;
        //     boxCenter.y -= _downBoxHeight / 2;
        //     Vector3 boxHalfSize = new Vector3(1f, _downBoxHeight, 1f) / 2;  // 캐스트할 박스 크기의 절반 크기. 이렇게 하면 가로 2 세로 2 높이 2의 크기의 공간을 캐스트한다.
        //     int layerMask = 1;
        //     layerMask = layerMask << LayerMask.NameToLayer("Ground");
        //     RaycastHit[] hits = Physics.BoxCastAll(boxCenter, boxHalfSize, Vector3.down, Quaternion.identity, 0.1f, layerMask);    // BoxCastAll은 찾아낸 충돌체를 배열로 반환한다.
        //     
        //     //if (hits.Length > 1)
        //     //{
        //     //    Debug.Log("=====");
        //     //    foreach (var hi in hits)
        //     //    {
        //     //        if (_downBoxHeight > Vector3.Distance(boxCenter, hi.transform.position))
        //     //            Debug.Log($"name : {hi.collider.name} distance : {Vector3.Distance(boxCenter, hi.transform.position)}");
        //     //    }
        //     //}
        //     if (hits.Length > 0)
        //     {
        //         //Debug.Log("Ground Box!!!");
        //         _isGround = true;
        //     }
        //
        //     // todo
        //     // 바닥으로 레이져 쏴서 모든 ground의 접점 거리 체크하기
        //     // 접점이 가장 짧은 길이가 height보다 작으면 바닥으로 취급시키기
        //     // >> 점프 시작 상태일 때 높이가 up 벡터 방향으로 이동 시 바닥을 무시하도록 변경
        //     // 
        //     // 점프 시작 시 바닥으로 변경하는 처리가 필요 
        //     // // + 기존에 0.2초 딜레이 후 바닥체크를 시작했었음...(JumpState 참고)
        //     // // + 시간 초 제거 후 JumpState 상태 시 y값이 최대값이 아니게 될 때부터 바닥 체크
        //     // // + fall 코드 한번만 더 생각해보자
        //
        //     //RaycastHit hit;
        //     //if (Physics.Raycast(curPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        //     //{
        //     //    Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.red);
        //     //    _isGround = false;
        //     //    if (hit.distance <= CHECK_GROUND_DISTANCE)
        //     //    {
        //     //        _isGround = true;
        //     //        Debug.Log("Ground Check!!!");
        //     //    }
        //     //}
        //     //else
        //     //{
        //     //    Debug.DrawRay(curPos, transform.TransformDirection(Vector3.down) * 1000, Color.white);
        //     //    _isGround = true;
        //     //    Debug.Log("Ground not ray!!!");
        //     //}
        // }
    }

    public override void EndState()
    {
        Debug.Log($"[State] jumpup end");
    }
}