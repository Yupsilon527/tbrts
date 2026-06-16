
public static class CombatDefines 
{
    public enum Events
    {
        Ticks = 0,
        HitsLanded = 1,
        HitsTaken = 2,
    }
    public enum ChanceMult
    {
        always = 0,
        True = 1,
        Luck = 2,
        Proc = 3,
    }
    public enum Targeting
    {
        none =0,
        self = 1,
        attackTarget = 2,
        randomEnemy = 3,
        allEnemies = 4,
        randomAlly =5,
        allAllies = 6,
        randomSecondaryTarget = 7,
    }
    public enum Action
    {
        // Procs
        Attack = 0,
        Ranged = 1 ,
        FirstStrike = 2,
        AttackOnce = 3,
        BuffAlly = 4,
        BuffSelf = 5,
        Huddle = 6,
        MagicAttack = 7,
        PowerAttack = 8,

        // Combat Events
        OnHit = 10,       //When attack lands
        OnParried = 11,     //when critical miss
        ParryAttack = 12,   //retaliates on critical miss 
        OnCrit = 13,      //critical hit
        OnMiss = 14,      //regular miss
        OnDodge = 15,     //when dodges attack
        EvadeAttack = 16,     //when dodge, or target misses
        Block = 17,     //when blocked, separate from dodge
        Proc = 18,      //proc elemental attacks/chance
        Retaliate = 19,   // when attacked


        // Misc Events
        ShieldBreak = 20,
        BelowHalf=21,
        BelowThird = 22,
        BelowQuarter = 23,
        EnemyBelowHalf=24,
        EnemyBelowThird = 25,
        EnemyBelowQuarter = 26,
        Enraged = 27,
        FinalStrike = 28,
        OnKill = 29,
        Crit = 30,
    }
    public static bool RequiresLearning(Action action)
    {
        return action == Action.Ranged
        || action == Action.FirstStrike
        || action == Action.BuffAlly
        || action == Action.Huddle
        || action == Action.MagicAttack
        || action == Action.PowerAttack
        || action == Action.BuffSelf;
    }
    public static bool IsSpell(Action action)
    {
        return action == Action.MagicAttack
        || action == Action.BuffAlly
        || action == Action.BuffSelf;
    }
}
