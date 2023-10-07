public class HitboxInfo
{
    private string _hitboxKey;
    private float _startRate;
    private float _endRate;
    private AttackType _attackType;
    private AttackRangeType _attackRangeType;
    public float damageRatio;
    public float attackHeight;
    public float airborneUpTime;
    
    public HitboxInfo()
    {
        _attackType = AttackType.NONE;
        attackHeight = 0f;
    }

    public HitboxInfo(string hitboxKey, AttackRangeType attackRangeType, float damageRatio, float startRate, float endRate, AttackType attackType, float attackHeight, float airborneUpTime)
    {
        _hitboxKey = hitboxKey;
        _attackRangeType = attackRangeType;
        this.damageRatio = damageRatio;
        _startRate = startRate;
        _endRate = endRate;
        _attackType = attackType;
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

    public string GetHitboxKey()
    {
        return _hitboxKey;
    }
}