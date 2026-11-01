
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
        OnDayBegin = 12,
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

        //End Of Battle
        OnEndTurn = 44,
        OnBeginTurn = 45,
        Victory = 46,

        //Out Of Battle
        OnRuinRest = 47,
        OnRuinHeal = 48,
        OnRuinPray = 49,
        OnVisitRuin = 50,
        OnHarvestRuin = 51,
        OnRuinBoon  = 52,

        Total = 53,
    }
    public enum ReactionEvent
    {
        UseDirectAttack,
        UseIndirectAttack,
        UseMagicAttack,

        UseSingleDirect,
        UseSingleIndirect,
         UseMultiDirect,
        UseMultiIndirect,

        UseSingleMagic,
        UseMultiMagic,

        HitBySingle,
        HitBySingleDirect,
        HitBySingleIndirect,
        HitByMulti,
        HitByMultiDirect,
        HitByMultiIndirect,
        HitByDirect,
        HitByIndirect,
        HitByMagic,

        OnKilled,
        OnScoreKill,

        OnBlock,
        OnCrit,
    }
    public enum ReactionTarget
    {
        self,
        ally,
        enemy,
    }
    public enum ReactionTiming
    {
        Before,
        Assist,
        After,
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
