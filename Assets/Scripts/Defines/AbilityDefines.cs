
public static class AbilityDefines
{

    public enum Event
    {
        Nothing = -1,
        Step = 29,

        //functional
        OnCreated = 0,
        OnDestroyed = 1,
        OnExpired = 2,
        OnRefresh = 3,
        OnStacksChange = 4,
        OnThink = 25,

        //combat
        OnSpawn = 5,
        CombatBegin = 6,
        CombatEnd = 7,

        OnScoreKill = 8,
        OnKilled = 9,

        BeforeAttack = 10,
        AttackHit = 11,
        AfterAttack = 12,
        OnHitByEnemy = 13,

        OnTakeDamage = 14,
        OnTakeLifeDamage = 15,
        OnLifeChange = 30,

        OnHealRecieved = 16,
        OnShieldRecieved = 17,
        OnArmorRecieved = 18,

        OnShieldBlock = 19,
        OnShieldBreak = 20,

        OnArmorBlock = 21,
        OnArmorBreak = 22,

        SuccessfulEvade = 26,
        SuccessfulBlock = 27,
        SuccessfulParry = 28,

        //on board
        OnMovementEnd = 23,
        OnLoopEnd = 24,

        Total = 31
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
