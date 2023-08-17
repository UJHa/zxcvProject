public class ActionInfo
{
    private ActionType _actionType;
    private string _clipPath;
    private float _startRate;
    private float _endRate;

    public ActionInfo(ActionType actionType, string clipPath, float startRate, float endRate)
    {
        _actionType = actionType;
        _clipPath = clipPath;
        _startRate = startRate;
        _endRate = endRate;
    }

    public float GetStartRate()
    {
        return _startRate;
    }
    
    public void SetStartRate(float argStartRate)
    {
        _startRate = argStartRate;
    }
    
    public float GetEndRate()
    {
        return _endRate;
    }
    
    public void SetEndRate(float argEndRate)
    {
        _endRate = argEndRate;
    }

    public float GetRateLength()
    {
        return _endRate - _startRate;
    }

    public string GetClipPath()
    {
        return _clipPath;
    }

    public ActionType GetActionType()
    {
        return _actionType;
    }
}