public class AttackInfo
{
    private float _startRate;
    private float _endRate;
    private AttackType _attackType;
    private AttackRangeType _attackRangeType;
    public float attackHeight;
    public float airborneUpTime;
    
    public AttackInfo()
    {
        _attackType = AttackType.NONE;
        attackHeight = 0f;
    }

    public AttackInfo(AttackRangeType attackRangeType, float startRate, float endRate, AttackType attackType, float attackHeight, float airborneUpTime)
    {
        _attackRangeType = attackRangeType;
        _startRate = startRate;
        _endRate = endRate;
        this._attackType = attackType;
        this.attackHeight = attackHeight;
        this.airborneUpTime = airborneUpTime;
    }

    public float GetStartRate()
    {
        return _startRate;
    }
    
    public float GetEndRate()
    {
        return _endRate;
    }

    public AttackRangeType GetRangeType()
    {
        return _attackRangeType;
    }

    public AttackType GetAttackType()
    {
        return _attackType;
    }
}