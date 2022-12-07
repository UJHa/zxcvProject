using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class JumpState : State
{
    private Stopwatch stopwatch;
    private long delayMillisec = 1000;
    private Direction _jumpDirection = Direction.FRONT;

    public JumpState(Character character) : base(character)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        character.StartJump();

        stopwatch.Reset();
        stopwatch.Start();

        _jumpDirection = character.GetDirection();

        Vector3 pos = character.transform.position;
        // pos.y += character._jumpOffset;
        // character.transform.position = pos;
        // character.GetRigidbody().velocity = character.GetJumpForce();
        // character.GetRigidbody().AddForce(character.GetJumpForce(), character.GetForceModeType());
        UnityEngine.Debug.Log($"=======Jump Start! velocity({character.GetRigidbody().velocity})");
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void EndState()
    {
        stopwatch.Stop();

        character.ResetPrevMoveSpeed();
    }

    public override void UpdateState()
    {
        if (delayMillisec <= stopwatch.ElapsedMilliseconds)
        {
            character.CheckIsGround();
            stopwatch.Stop();
        }

        // 이건 여기서 처리하면 이제 안됨(State 별 체크?)
        if (character.IsGround())
        {
            character.ChangeState(eState.IDLE);
        }
        character.MoveDirectionPosition(_jumpDirection);

        foreach (Direction direction in character.GetDirections())
        {
            if (character.GetKeysDirection(direction))
            {
                character.SetDirection(direction);
                break;
            }
        }
    }
}