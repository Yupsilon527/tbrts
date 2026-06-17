using System.Collections.Generic;
using System.Linq;

public class Combat : Initializable
{
    public static Combat main;
    protected override void Initialize()
    {
        if (main == null)
            main = this;
        base.Initialize();
    }

    public CombatDefines.AttackPhase currentPhase;
    public SidewaysTile locatedTile;
    public DataItemArmy attackers, defenders;

    public List<DataItemUnit> combatants = new();

    public int currentTick = 0;
    public void SetUp(DataItemArmy a, DataItemArmy d, SidewaysTile location)
    {
        currentTick = 0;
        locatedTile = location;
        attackers = a;
        defenders = d;

        InitCombatants();
    }
    void InitCombatants()   //TODO ranged support
    {
        foreach (var a in attackers.Formation)
        {
            combatants.Add(a);
        }
        combatants.Add(attackers.transport);

        foreach (var d in defenders.Formation)
        {
            combatants.Add(d);
        }
        combatants.Add(defenders.transport);
    }
    void BeginCombat()
    {
        enabled = true;
        currentTick = 0;
        FireEventOnAllFighters(AbilityDefines.Event.CombatBegin);
        currentPhase = CombatDefines.AttackPhase.Prep;
    }
    public bool IsInCombat()
    {
        return enabled;
    }
    void ResolveInstantly()
    {
        BeginCombat();
        int soft = 0;
        while (enabled && ++soft < 100)
        {
            OnGameTick();
        }
    }
    private void Update()
    {
        OnGameTick();
    }
    public void OnGameTick()
    {
        if (!IsInCombat()) return;
        combatants.Sort((a, b) => a.nextAction.CompareTo(b.nextAction));
        currentTick = combatants[0].nextAction;

        while (combatants[0].nextAction <= currentTick)
        {
            Inspect($"{combatants[0]} acts!");
            combatants[0].Act();
            combatants.Sort((a, b) => a.nextAction.CompareTo(b.nextAction));

            if (combatants.Any(c => c.abilities._actions.Any(a => a.CanBeCast(currentPhase))))
                continue;
            ForwardPhase();
        }
    }
    void ForwardPhase()
    {
        if (currentPhase == CombatDefines.AttackPhase.PostAttack)
            EndCombat();
        else
        {
            currentPhase++;
            currentTick = 0;
        }
    }
    void EndCombat()
    {
        FireEventOnAllFighters(AbilityDefines.Event.CombatEnd);
        combatants.Clear();
        enabled = false;
    }
    void FireEventOnAllFighters(AbilityDefines.Event e)
    {
        foreach (var combatant in combatants)
        {
            combatant.FireEventOnSelf(e, true);
        }
    }
}
