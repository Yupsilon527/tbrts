
public static class AbilityDefines
{
    public enum Condition
    {
        Always,
        Damaged,
        Alive,
    }
    public enum Event
    {
        Nothing = -1,

        OnCreated = 0,
        OnDestroyed = 1,
        OnExpired = 2,
        OnRefresh = 3,
        OnStacksChange = 4,

        Time = 5,
        Action = 6,

        OnSpawn = 7,

        // Combat Flow
        CombatBegin = 8,
        CombatEnd = 9,
        CombatExit = 10,
        CombatPhase = 11,

        // Turn / Board
        OnTurnBegin = 12,
        OnMoveTile = 13,

        // Aura / Stack
        OnUnitEnterStack = 14,
        OnUnitExitStack = 15,

        // Attacks & Hits
        BeforeAttack = 16,
        BeforeDirectAttack = 17,
        BeforeIndirectAttack = 18,
        BeforeMagicAttack = 19,

        AfterAttack = 20,
        AttackHit = 21,

        OnHitEnemy = 22,
        OnHitByEnemy = 23,
        DirectHitByEnemy = 24,
        IndirectHitByEnemy = 25,

        OnCritEnemy = 26,
        OnDodgeEnemy = 27,
        OnBlockEnemy = 28,

        // Kill Events
        OnScoreKill = 29,
        OnKilled = 30,

        // Spells
        CastSpell = 31,
        OnHitSpell = 32,
        OnHitBySpell = 33,

        // Damage & Healing
        OnTakeDamage = 34,
        OnTakeLifeDamage = 35,
        OnLifeChange = 36,

        OnHealReceived = 37,
        OnShieldReceived = 38,
        OnArmorReceived = 39,

        // Defense Reactions
        OnShieldBlock = 40,
        OnShieldBreak = 41,
        OnArmorBlock = 42,
        OnArmorBreak = 43,

        // Ally Reactions
        BeforeAllyUseDirectAttack = 44,
        BeforeAllyUseIndirectAttack = 45,

        // Enemy Reactions
        OnEnemyUseDirectAttack = 46,
        OnEnemyUseIndirectAttack = 47,
        OnEnemyUseMagicAttack = 48,
        OnEnemyUseSingleAttack = 49,
        OnEnemyUseMultiAttack = 50,

        // Specific Hit Types
        OnHitBySingleIndirect = 51,
        OnHitBySingleDirect = 52,
        OnHitByMultiIndirect = 53,
        OnHitByMultiDirect = 54,
        OnHitByMagicAttack = 55,

        AssistSingleIndirect = 56,
        AssistSingleDirect = 57,
        AssistSingleMagic = 58,

        // Specific Hit Types
        AfterAllyHitBySingle = 59,
        AfterAfterHitByMulti = 60,

        AfterAfterHitByDirect = 61,
        AfterAfterHitByIndirect = 62,
        AfterAfterHitByMagic = 63,

        // Total (always last)
        Total = 64,
    }

    public delegate void AbilityFunction(EventTable CastData);
    public class AbilityListener
    {
        public Event aEvent;
        public AbilityFunction aFunction;

        public AbilityListener(AbilityEvent evt, DataItemUnit activator)
        {
            aEvent = evt.Event;
            aFunction = (EventTable castData) =>
            {
                foreach (var effect in evt.defaultEffects)
                {
                    effect?.ActivateOnUnit(castData);
                }
            };
        }
        public AbilityListener(Event aEvent, AbilityFunction aFunction)
        {
            this.aEvent = aEvent;
            this.aFunction = aFunction;
        }

    }
    [System.Serializable]
    public class AbilityEvent
    {
        public Event Event;
        public ApplyEffects[] defaultEffects = new ApplyEffects[0];
    }
}
