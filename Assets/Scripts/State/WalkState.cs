using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class WalkState : State
{
    private Stopwatch stopwatch;
    private long delayMillisec = 150;
    private bool isAllKeyUp = false;
    private bool _isJump = false;

    public WalkState(Character character) : base(character)
    {
        stopwatch = new Stopwatch();
    }

    public override void StartState()
    {
        _isJump = false;

        character.SetMoveSpeedToWalk();
        stopwatch.Reset();
        isAllKeyUp = false;

        animator.SetBool("Walk", true);
    }

    public override void EndState()
    {
        stopwatch.Stop();

        if (!_isJump)
            animator.SetBool("Walk", false);
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.V) && character.IsGround())
        {
            _isJump = true;
            character.ChangeState(eState.JUMP);
            return;
        }

        if (isAllKeyUp)
        {
            if (delayMillisec <= stopwatch.ElapsedMilliseconds)
            {
                character.ChangeState(eState.IDLE);
                return;
            }
            else
            {
                foreach (Direction direction in character.GetDirections())
                {
                    if (character.GetKeysDownDirection(direction) 
                        && character.GetDirection() == direction)
                    {
                        character.ChangeState(eState.RUN);
                        return;
                    }

                    if (character.GetKeysDownDirection(direction))
                    {
                        isAllKeyUp = false;
                        stopwatch.Stop();
                        stopwatch.Reset();
                    }
                }
            }
        }

        bool isInput = false;

        foreach (Direction direction in character.GetDirections())
        {
            if (character.GetKeysDirection(direction))
            {
                character.SetDirection(direction);

                isInput = true;
                break;
            }
        }

        if (isInput == false)
        {
            isAllKeyUp = true;
            stopwatch.Start();
        }
        else
        {
            character.MoveDirectionPosition(character.GetDirection());
        }
    }
}