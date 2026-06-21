using System;
using System.Collections.Generic;
using System.Linq;

public class UnitBonuses : UnitComponent
{
    public UnitBonuses(DataItemUnit parent) : base(parent)
    {
        unitFlags = parent.data.unitFlags;
    }
    public AttackDefines.MobFlag[] unitFlags;

    public HashSet<BonusDamageTable> bonusDamage = new();

    public void GrantBonusDamageFromTable(BonusDamageTable[] data, int oldLevel, int newLevel)
    {
        foreach (var table in data)
        {
            GrantBonusDamageFromTable(table, oldLevel, newLevel);
        }
    }
    public BonusDamageTable GetBonusAgainstUnit(AttackDefines.MobFlag flag, AttackDefines.BonusType scale)
    {
        var valid = bonusDamage.FirstOrDefault(bonus => bonus.flag == flag && bonus.scale == scale);
        if (valid == null)
            return new BonusDamageTable(scale == AttackDefines.BonusType.additive ? 0 : 1, scale, flag);
        return valid;
    }
    public void GrantBonusDamageFromTable(BonusDamageTable table, int oldLevel, int newLevel)
    {
        int deltaLevel = newLevel - oldLevel;
        if (bonusDamage.Any(bonus => bonus.flag == table.flag && bonus.scale == table.scale))
        {
            GetBonusAgainstUnit(table.flag, table.scale).damage += table.damage * deltaLevel;
        }
        else
        {
            bonusDamage.Add(new BonusDamageTable((table.scale == AttackDefines.BonusType.additive ? 0 : 1) + table.damage * deltaLevel, table.scale, table.flag));
        }
    }
    public float CalculateDamageAgainstTarget(DataItemUnit target, float baseDamage)
    {
        float raw = 0;
        float mult = 1;
        foreach (var flag in target.bonuses.unitFlags)
        {
            raw += GetBonusAgainstUnit(flag, AttackDefines.BonusType.additive).damage;
            mult *= GetBonusAgainstUnit(flag, AttackDefines.BonusType.multiplicative).damage;
        }
        return (baseDamage + raw) * mult;
    }
}

[Serializable]
public class BonusDamageTable
{
    public float damage = 0;
    public AttackDefines.BonusType scale;
    public AttackDefines.MobFlag flag;

    public BonusDamageTable() { }
    public BonusDamageTable(float damage, AttackDefines.BonusType scale, AttackDefines.MobFlag flag)
    {
        this.damage = damage;
        this.scale = scale;
        this.flag = flag;
    }
}