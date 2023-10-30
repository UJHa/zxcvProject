using DataClass;
using Utils;

public class HitboxInfo
{
    private ActionKey _state;
    private float _startRate;
    private float _endRate;
    private AttackType _attackType;
    private HitboxType _hitboxType;
    public float damageRatio;
    public float attackHeight;
    public float airborneUpTime;
    
    public HitboxInfo(AttackInfoData attackInfoData)
    {
        _hitboxType = UmUtil.StringToEnum<HitboxType>(attackInfoData.hitboxType);
        damageRatio = attackInfoData.damageRatio;
        _startRate = attackInfoData.startRatio;
        _endRate = attackInfoData.endRatio;
        _attackType = UmUtil.StringToEnum<AttackType>(attackInfoData.attackType);
        attackHeight = attackInfoData.airborneHeight;
        airborneUpTime = attackInfoData.airborneTime;
    }

    public float GetStartRate()
    {
        return _startRate;
    }
    
    public float GetEndRate()
    {
        return _endRate;
    }

    public HitboxType GetRangeType()
    {
        return _hitboxType;
    }

    public AttackType GetAttackType()
    {
        return _attackType;
    }

    public ActionKey GetState()
    {
        return _state;
    }
}