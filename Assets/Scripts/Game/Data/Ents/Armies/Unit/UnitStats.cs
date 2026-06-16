
public class UnitStats : UnitComponent
{
    #region Stats
    public UnitStatsTable baseStats = new();
    public UnitStatsTable realStats = new();
    public void ResetStats()
    {
        realStats = baseStats.Clone();
    }
    #endregion
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        if (act == AbilityDefines.Event.CombatBegin
            || act == AbilityDefines.Event.OnRefresh)
        {
            Recalculate();
        }
    }
    public UnitStats(DataItemUnit owner, UnitStatsTable stats) : base(owner)
    {
        LoadStatsTable(stats);

    }
    public void LoadStatsTable(UnitStatsTable scriptable)
    {
        if (scriptable is null)
        {
            return;
        }
        baseStats = scriptable.Clone();
        ResetStats();
    }

    public void Recalculate()
    {
        UpdateOffense();
        UpdateDefense();

        UpdateAttack();
        UpdateMagic();
        UpdateAction();

        UpdateArmor();
        UpdateBlock();
        UpdateResist();

        UpdateSpeed();
        UpdateLuck();

        UpdateMaxHealth();
        UpdateBarrier();
    }
    #region Combat
    public virtual void UpdateOffense()
    {
        realStats.Offense = baseStats.Offense;
        realStats.Offense += parent.GetProperty(ModifierDefines.Properties.bonus_offense);
        realStats.Offense += parent.GetProperty(ModifierDefines.Properties.bonus_combat);
    }
    public virtual void UpdateDefense()
    {
        realStats.Defense = baseStats.Defense;
        realStats.Defense += parent.GetProperty(ModifierDefines.Properties.bonus_defense);
        realStats.Defense += parent.GetProperty(ModifierDefines.Properties.bonus_combat);
    }
    #endregion
    #region Defensive
    public virtual void UpdateArmor()
    {
        realStats.Armor = baseStats.Armor;
        realStats.Armor += parent.GetProperty(ModifierDefines.Properties.armor_bonus);
        realStats.Armor *= parent.GetProperty(ModifierDefines.Properties.armor_bonus_percent);
    }
    public virtual void UpdateBlock()
    {
        realStats.Block = baseStats.Block;
        realStats.Block += parent.GetProperty(ModifierDefines.Properties.block_bonus);
        realStats.Block *= parent.GetProperty(ModifierDefines.Properties.block_bonus_percent);
    }
    public virtual void UpdateResist()
    {
        realStats.Armor = baseStats.Armor;
        realStats.Armor += parent.GetProperty(ModifierDefines.Properties.resistance_bonus);
        realStats.Armor *= parent.GetProperty(ModifierDefines.Properties.resistance_bonus_percent);
    }
    #endregion
    #region Attack
    public virtual void UpdateAttack()
    {
        realStats.Attack = baseStats.Attack;
        realStats.Attack += parent.GetProperty(ModifierDefines.Properties.attack_bonus);
        realStats.Attack *= parent.GetProperty(ModifierDefines.Properties.attack_bonus_percent);
    }
    public virtual void UpdateMagic()
    {
        realStats.Magic = baseStats.Magic;
        realStats.Magic += parent.GetProperty(ModifierDefines.Properties.special_bonus);
        realStats.Magic *= parent.GetProperty(ModifierDefines.Properties.special_bonus_percent);
    }
    #endregion
    #region Endurance
    public virtual void UpdateMaxHealth()
    {
        realStats.Health = baseStats.Health;
        realStats.Health += parent.GetProperty(ModifierDefines.Properties.health_bonus);
        realStats.Health *= parent.GetProperty(ModifierDefines.Properties.health_bonus_percent);
        if (parent.damageable != null)
        {
            parent.damageable.Health.SetValue(realStats.Health);
        }
    }
    public virtual void UpdateBarrier()
    {
        realStats.Barrier = baseStats.Barrier;
        realStats.Barrier += parent.GetProperty(ModifierDefines.Properties.barrier_bonus);
        realStats.Barrier *= parent.GetProperty(ModifierDefines.Properties.barrier_bonus_percent);
        if (parent.damageable != null)
        {
            parent.damageable.Armor.SetLimit(realStats.Barrier, rule: Resource.LimitRule.give_difference);
        }
    }
    public virtual void UpdateAction()
    {
        realStats.Action = baseStats.Action;
        realStats.Action += parent.GetProperty(ModifierDefines.Properties.action_bonus);

        parent.abilities?.Ap?.SetLimit(realStats.Action, Resource.LimitRule.leave_value);
    }
    public virtual void UpdateMana()
    {
        realStats.Mana = baseStats.Mana;
        realStats.Mana += parent.GetProperty(ModifierDefines.Properties.mana_bonus);

        parent.abilities?.Mp?.SetLimit(realStats.Mana, Resource.LimitRule.leave_value);
    }
    #endregion
    #region Misc Stats
    public virtual void UpdateLuck()
    {
        realStats.Luck = baseStats.Luck;
        realStats.Luck += parent.GetProperty(ModifierDefines.Properties.luck_bonus);
        realStats.LuckCoefficient += realStats.GetLuckCoefficient();
    }
    public virtual void UpdateSpeed()
    {
        realStats.Speed = baseStats.Speed;
        realStats.Speed += parent.GetProperty(ModifierDefines.Properties.luck_bonus);
        realStats.SpeedCoefficient += realStats.GetSpeedMultiplier();
    }
    public virtual void UpdateMisc()
    {
        realStats.DodgeChance = baseStats.DodgeChance + parent.GetProperty(ModifierDefines.Properties.dodge_chance);
        realStats.BlockChance = baseStats.BlockChance + parent.GetProperty(ModifierDefines.Properties.block_chance);
        realStats.ProcChance = baseStats.ProcChance + parent.GetProperty(ModifierDefines.Properties.proc_chance);
    }
    #endregion

}