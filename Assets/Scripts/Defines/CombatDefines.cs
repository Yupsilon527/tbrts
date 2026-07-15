
public static class CombatDefines
{
    #region Attacks
    public enum AttackPhase
    {
        BeforeCombat = 0,
        Prep = 1,
        Attack = 2,
        Parting = 3,
        AfterCombat = 4,
        OutOfCombat = 5,
    }
    public enum AttackFlag
    {
        nothing = 0,
        targetAllies = 1 << 1,
        castInFrontRow = 1 << 2,
        castInBackRow = 1 << 3,
        castInTransport = 1 << 4,
        castInSupport = 1 << 5,
        targetFrontRow = 1 << 6,
        targetBackRow = 1 << 7,
        targetOwnCol = 1 << 8,
        targetSelf = 1 << 9,
        cannotMiss = 1 << 10,
        indirectAttack = 1 << 11,
        usableOnce = 1 << 12,
        magicAttack = 1 << 13,
    }
    public enum ArmyPriorityMode
    {
        random
    }
    public enum CombatantTargetingArea
    {
        single,
        row,
        column,
        all,
    }
    #endregion
    #region Spells
    public enum SpellFlag
    {
        nothing = 0,
        targetAllies = 1 << 1,
        targetEnemies = 1 << 2,
        mustNotHaveActed = 1 << 3,
    }
    public enum AbilityCastMode
    {
        passive,
        self,
        point,
        random_tile,
    }
    public enum TileTargetingMode
    {
        circle,
        square,
        cross,
        diagcross,
        cross8,
    }
    public enum TileTargetingArea
    {
        tile,
        circle,
        square,
        line,
        line8,
        cone_narrow,
        cone,
        cone8,
        cross,
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
