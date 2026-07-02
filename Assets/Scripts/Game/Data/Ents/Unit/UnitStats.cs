
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
            || act == AbilityDefines.Event.OnRefresh
            || act == AbilityDefines.Event.OnSpawn)
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
        UpdateMana();
        UpdateSupply();

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
        realStats.Offense += parent.GetProperty(ModifierDefines.Property.bonus_offense);
        realStats.Offense += parent.GetProperty(ModifierDefines.Property.bonus_combat);
    }
    public virtual void UpdateDefense()
    {
        realStats.Defense = baseStats.Defense;
        realStats.Defense += parent.GetProperty(ModifierDefines.Property.bonus_defense);
        realStats.Defense += parent.GetProperty(ModifierDefines.Property.bonus_combat);
    }
    #endregion
    #region Defensive
    public virtual void UpdateArmor()
    {
        realStats.Armor = baseStats.Armor;
        realStats.Armor += parent.GetProperty(ModifierDefines.Property.armor_bonus);
        realStats.Armor *= parent.GetProperty(ModifierDefines.Property.armor_bonus_percent);
    }
    public virtual void UpdateBlock()
    {
        realStats.Block = baseStats.Block;
        realStats.Block += parent.GetProperty(ModifierDefines.Property.block_bonus);
        realStats.Block *= parent.GetProperty(ModifierDefines.Property.block_bonus_percent);
    }
    public virtual void UpdateResist()
    {
        realStats.Armor = baseStats.Armor;
        realStats.Armor += parent.GetProperty(ModifierDefines.Property.resistance_bonus);
        realStats.Armor *= parent.GetProperty(ModifierDefines.Property.resistance_bonus_percent);
    }
    #endregion
    #region Attack
    public virtual void UpdateAttack()
    {
        realStats.Attack = baseStats.Attack;
        realStats.Attack += parent.GetProperty(ModifierDefines.Property.attack_bonus);
        realStats.Attack *= parent.GetProperty(ModifierDefines.Property.attack_bonus_percent);
    }
    public virtual void UpdateMagic()
    {
        realStats.Magic = baseStats.Magic;
        realStats.Magic += parent.GetProperty(ModifierDefines.Property.special_bonus);
        realStats.Magic *= parent.GetProperty(ModifierDefines.Property.special_bonus_percent);
    }
    #endregion
    #region Endurance
    public virtual void UpdateMaxHealth()
    {
        realStats.Health = baseStats.Health;
        realStats.Health += parent.GetProperty(ModifierDefines.Property.health_bonus);
        realStats.Health *= parent.GetProperty(ModifierDefines.Property.health_bonus_percent);
        if (parent.damageable != null)
        {
            parent.damageable.Health.SetLimit(realStats.Health, Resource.LimitRule.percent_value);
        }
        parent.health.SetLimit(realStats.Health, Resource.LimitRule.percent_value);
    }
    public virtual void UpdateBarrier()
    {
        realStats.Barrier = baseStats.Barrier;
        realStats.Barrier += parent.GetProperty(ModifierDefines.Property.barrier_bonus);
        realStats.Barrier *= parent.GetProperty(ModifierDefines.Property.barrier_bonus_percent);
        if (parent.damageable != null)
        {
            parent.damageable.Armor.SetLimit(realStats.Barrier, rule: Resource.LimitRule.give_difference);
        }
    }
    public virtual void UpdateAction()
    {
        realStats.Action = baseStats.Action;
        realStats.Action += parent.GetProperty(ModifierDefines.Property.action_bonus);

        parent.actions?.Ap?.SetLimit(realStats.Action, Resource.LimitRule.leave_value);
    }
    public virtual void UpdateMana()
    {
        realStats.Mana = baseStats.Mana;
        realStats.Mana += parent.GetProperty(ModifierDefines.Property.mana_bonus);

        parent.actions?.Mp?.SetLimit(realStats.Mana, Resource.LimitRule.leave_value);
    }
    public virtual void UpdateSupply()
    {
        realStats.Supply = baseStats.Supply;
        realStats.Supply += parent.GetProperty(ModifierDefines.Property.supply_bonus);

        parent.actions?.Sp?.SetLimit(realStats.Supply, Resource.LimitRule.leave_value);
    }
    #endregion
    #region Misc Stats
    public virtual void UpdateLuck()
    {
        realStats.Luck = baseStats.Luck;
        realStats.Luck += parent.GetProperty(ModifierDefines.Property.luck_bonus);
        realStats.LuckCoefficient += realStats.GetLuckCoefficient();
    }
    public virtual void UpdateSpeed()
    {
        realStats.Speed = baseStats.Speed;
        realStats.Speed += parent.GetProperty(ModifierDefines.Property.luck_bonus);
        realStats.SpeedCoefficient += realStats.GetSpeedMultiplier();
    }
    public virtual void UpdateMisc()
    {
        realStats.DodgeChance = baseStats.DodgeChance + parent.GetProperty(ModifierDefines.Property.dodge_chance);
        realStats.BlockChance = baseStats.BlockChance + parent.GetProperty(ModifierDefines.Property.block_chance);
        realStats.ProcChance = baseStats.ProcChance + parent.GetProperty(ModifierDefines.Property.proc_chance);
    }
    #endregion

}