using System;

[Serializable]
public class ApplyAttack : ApplyEffects
{
    public AttackDefines.AttackType attack;
    public float BaseDamage = 0;
    public ScaleData[] scaling;

    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        float realDamage = BaseDamage * strength;
        foreach (var scale in scaling)
        {
            realDamage = scale.GetScaleStrength(table.caster, table.GetTarget(), realDamage);
        }
        Combat.main.Inspect($"{table.caster} deals {realDamage * strength} base damage to {table.GetTarget()}");
        table.GetTarget().damageable.DealDamage(realDamage * strength , attack);
    }

    public override string GetDescription()
    {
        string effect = $"{BaseDamage} {attack}";

        return base.GetDescription()
            .Replace("%effect%", "deal " + effect);
    }
}
[Serializable]
public class ScaleData
{
    public AttackDefines.ScaleType scaleMode;
    public float scaleDamage = 0;

    public AttackDefines.ScaleMode scaleoff = AttackDefines.ScaleMode.caster;
    public AttackDefines.ScaleRate scaleRate;
    public float GetScaleStrength(DataItemUnit attacker, DataItemUnit target, float baseDamage)
    {
        return AttackDefines.GetScaleStrength(attacker, target, scaleoff,baseDamage,scaleMode,scaleRate,scaleDamage);
    }
}