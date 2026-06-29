
public static class CombatDefines
{
    #region Attacks
    public enum AttackPhase
    {
        Prep = 0,
        Attack = 1,
        PostAttack = 2,
    }
    public enum AttackFlag
    {
        nothing = 0,
        targetAllies = 1 << 1,
        castInFrontRow = 1 << 2,
        castInBackRow = 1 << 3,
        castInTransport = 1 << 4,
        castInRangeSupport = 1 << 5,
        targetsDirectlyInFront = 1 << 6,
    }
    public enum ArmyPriorityMode
    {
        random
    }
    public enum CombatantTargetingArea
    {
        tile,
        row,
        column,
        all,
    }
    public enum CombatantRangeMode
    {
        self,
        enemy_frontrow,
        enemy_backrow,
        enemy_ranged,
        enemy_transport,
        ally_frontrow,
        ally_backrow,
        ally_forward,
        ally_samerow,
    }
    #endregion
    #region Spells
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
    #endregion
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
    public enum ChanceMult
    {
        always = 0,
        True = 1,
        Luck = 2,
        Proc = 3,
    }
}
