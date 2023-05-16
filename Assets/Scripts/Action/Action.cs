// Action은 하나의 Monobavior가 가진 Animator 기반으로 동작된다고 가정한다

// 객체 예상 스펙
// >> 다음 액션 객체
// >> 애니메이션 이름
// >> 애니메이션 입력 가능 시간 범위
// >> 애니메이션 타입(이후 콤보 가능 액션, 입력 가능 액션, 타격 가능 액션 등...)(개발 전까지는 후순위)
// >> 다음 애니메이션 이동 시의 fade 시간 여부?(개발 전까지는 후순위)

using UnityEngine;

public class Action
{
    private readonly string _name;
    private readonly MoveSet _moveSet;
    private readonly KeyCode _inputKey;
    private readonly eState _state;

    public Action(string name, KeyCode inputKey, eState state, MoveSet moveSet)
    {
        _name = name;
        _moveSet = moveSet;
        _inputKey = inputKey;
        _state = state;
    }

    public eState GetState()
    {
        return _state;
    }
}