using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CGCharacter : MonoBehaviour
{
    private float _fullHp = 100f;
    private float _curHp = 0f;

    private float _moveSpeed = 0.03f;

    protected Dictionary<eCGState, CGState> stateMap = new Dictionary<eCGState, CGState>();

    private eCGState _prevState;
    public eCGState _curState;

    private Vector3 _targetMovePosition;

    public GameObject testCube;


    protected virtual void UpdateUI()
    {
    }

    private void Start()
    {
        _targetMovePosition = transform.position;
        _targetMovePosition.y = 0f;

        stateMap.Add(eCGState.IDLE, new CGPlayerIdle(this));
        stateMap.Add(eCGState.MOVE, new CGPlayerMove(this));
    }

    // Update is called once per frame
    void Update()
    {
        _prevState = _curState;
        stateMap[_curState].UpdateState();
        if (_prevState != _curState)
        {
            stateMap[_prevState].EndState();
            stateMap[_curState].StartState();
        }

        if (Input.GetMouseButtonDown(0))
        {
            //_targetMovePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //testCube.transform.position = _targetMovePosition;

            //if (Physics.Raycast(ray, out hit))
            //{
            //    Transform objectHit = hit.transform;

            //    _targetMovePosition = objectHit.position;
            //}
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                _targetMovePosition = hit.point;
                // for testcube
                _targetMovePosition.y = 3f;
                testCube.transform.position = _targetMovePosition;

                _targetMovePosition.y = 0f;
            }

            Debug.Log(_targetMovePosition);
        }

        UpdateUI();
    }

    public void ChangeState(eCGState state)
    {
        _curState = state;
    }

    public eCGState GetPrevState()
    {
        return _prevState;
    }

    public bool ArriveMousePoint()
    {
        Vector3 characterPos = transform.position;
        characterPos.y = 0.0f;
        return Vector3.Magnitude(characterPos - _targetMovePosition) < 0.04f;
    }

    public void SetDirection()
    {
        Vector3 characterPos = transform.position;
        characterPos.y = 0.0f;
        Debug.DrawRay(characterPos, _targetMovePosition - characterPos, Color.red);
        transform.rotation = Quaternion.LookRotation(_targetMovePosition - characterPos);
        //_targetMovePosition - characterPos
    }
    public void MovePosition()
    {
        Vector3 characterPos = transform.position;
        characterPos.y = 0.0f;
        Vector3 moveVector = _targetMovePosition - characterPos;
        transform.position += moveVector.normalized * _moveSpeed;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        //Vector3 vector3 = transform.position;
        //vector3.y += 0.9f;
        //Gizmos.DrawSphere(vector3, findRange);
    }
}