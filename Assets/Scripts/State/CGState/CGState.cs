using System.Collections;
using UnityEngine;

public enum eCGState
{
    IDLE,
    MOVE,
    ATTACK,
    DEAD
}
public abstract class CGState
{
    protected CGCharacter character;
    protected Animator animator;

    public CGState(CGCharacter character)
    {
        this.character = character;
        this.animator = character.GetComponent<Animator>();
    }

    public abstract void StartState();
    public abstract void UpdateState();
    public abstract void EndState();
}