
public static class AbilityDefines
{
    public enum Event
    {
        Nothing = -1,
        Time = 29,

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
        CombatExit = 34,    //exits a non mock-battle combat, used for "lasts for X fights" modifiers
        CombatPhase = 33,

		//board
        OnTurnBegin = 24,
        OnMoveTile = 23,

		//aura
		OnUnitEnterStack = 24,
		OnUnitExitStack = 25,

        OnScoreKill = 8,
        OnKilled = 9,

        BeforeAttack = 10,
        AttackHit = 11,

        OnHitByEnemy = 13,
        CastSpell = 12,
        OnHitSpell = 31,
        OnHitBySpell = 32,

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

        Total = 35
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

		public static int GetArmyBaseLoS = 4;
    }
    [System.Serializable]
    public class AbilityEvent
    {
        public Event Event;
        public ApplyEffects[] defaultEffects = new ApplyEffects[0];
    }
}
