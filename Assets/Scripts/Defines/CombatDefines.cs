
public static class CombatDefines
{
    public enum AttackPhase
    {
        OutOfCombat = 0,
        Prep = 1,
        Attack = 2,
        PostAttack = 3,
    }
    public enum ChanceMult
    {
        always = 0,
        True = 1,
        Luck = 2,
        Proc = 3,
    }
    public enum TargetType
    {
        nobody = -1,
        caster = 0,
        all_targets = 1,
        main_target = 2,
        side_targets = 3,
        randomEnemy = 4,
        randomAlly = 5,
    }
    public enum TileTargetingMode
    {
        passive,
        none,
        direction,
        tile,
        random_tile,
        random_closest_tile,
        random_farthest_tile,
    }
    public enum TileRangeMode
    {
        circle,
        cross,
        diagcross,
        star,
        square,
    }
    public enum TileTargetingArea
    {
        circle,
        square,
        line,
        cone,
    }
    public enum ArmyRangeMode
    {
        passive,
        frontrow,
        backrow,
        ranged,
        transport,
    }
    public enum ArmyTargetingArea
    {
        tile,
        row,
        column,
        all,
    }
}
