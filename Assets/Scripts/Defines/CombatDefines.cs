
public static class CombatDefines
{
    public enum AttackPhase
    {
        Prep = 0,
        Attack = 1,
        PostAttack = 2,
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
        targets = 1,
        caster_and_target = 2,
        none =0,
        self = 1,
        attackTarget = 2,
        randomEnemy = 3,
        allEnemies = 4,
        randomAlly =5,
        allAllies = 6,
        randomSecondaryTarget = 7,
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
