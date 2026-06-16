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
    public void StartCombat(DataItemArmy a, DataItemArmy d, SidewaysTile location)
    {
        currentTick = 0;
        locatedTile = location;
        attackers = a;
        defenders = d;

        InitCombatants();

        BeginCombat();
    }
    void InitCombatants()
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
    public  void OnGameTick(int steps)
    {
        if (!IsInCombat()) return;
        combatants.Sort((a, b) => a.nextAction.CompareTo(b.nextAction));
        currentTick = combatants[0].nextAction ;

        while (combatants[0].nextAction <= currentTick)
        {
            combatants[0].Act();
            combatants.Sort((a, b) => a.nextAction.CompareTo(b.nextAction));
            if (!ForwardCheck())
                break;
        }
    }
    bool ForwardCheck()
    {
        bool playerAlive = attackers.CountLivingTroops()>0;
        bool enemiesAlive = defenders.CountLivingTroops()>0;
        if (!playerAlive || !enemiesAlive)
        {
            DeclareWinner(playerAlive);
            return false;
        }
        return true;
    }
    void DeclareWinner(bool playerside)
    {
        Inspect(playerside ? "Player won" : "Monsters won");
        EndCombat();
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
