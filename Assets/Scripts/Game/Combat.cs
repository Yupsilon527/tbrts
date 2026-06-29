using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combat : Initializable
{
    public static Combat main;
    protected override void Initialize()
    {
        if (main == null)
            main = this;
        base.Initialize();
    }

    public SparseIntMap MockBattle(DataItemArmy a, DataItemArmy d, SidewaysTile l)
    {
        SetUp(a, d, l);
        ResolveInstantly();
        return OutputResults();
    }

    public CombatDefines.AttackPhase currentPhase;
    public SidewaysTile locatedTile;
    public DataItemArmy attackers, defenders;
    public bool mockBattle = false;

    public List<DataItemUnit> combatants = new();

    public int currentTick = 0;
    public void SetUp(DataItemArmy a, DataItemArmy d, SidewaysTile l)
    {
            Inspect($"COMBAT - Begin combat between army {a} and army {d} on tile {l}!");
        locatedTile = l;
        attackers = a;
        defenders = d;

        InitCombatants();
    }
    void InitCombatants()   //TODO ranged support
    {
        combatants.Clear();
        combatants.AddRange(attackers.formation.GetUnits());
        combatants.AddRange(defenders.formation.GetUnits());
            Inspect($"COMBAT - Loaded {combatants.Count} combatants for combat!");
    }
    void BeginCombat()
    {
            Inspect($"COMBAT - Begin Combat!");
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
        EndCombat();
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
            Inspect($"{combatants[0]} acts at tick {currentPhase}!");
            combatants[0].Act();
            combatants.Sort((a, b) => a.nextAction.CompareTo(b.nextAction));

            if (combatants.Any(c => c.abilities.abilities.Any(a => a.CanBeCast(currentPhase))))
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
        if (enabled) { 
        FireEventOnAllFighters(AbilityDefines.Event.CombatEnd);
        enabled = false;
    }
    }
    void FireEventOnAllFighters(AbilityDefines.Event e)
    {
            Inspect($"Event on all - {e}");
        foreach (var combatant in combatants)
        {
            combatant.FireEventOnSelf(e, true);
        }
    }
    public bool IsAttackingSide(DataItemUnit unit)
    {
        return unit.GetPlayerOwner() == attackers.GetPlayerOwner();
    }
    public DataItemUnit[] GetTroopsInSide(bool attacking)
    {
        return combatants.Where(u => IsAttackingSide(u) == attacking).ToArray();
    }
    public DataItemUnit GetUnitAt(bool attackingSide, int X, int Y)
    {
        return (attackingSide ? attackers : defenders).formation.GetTroopInPosition(X, Y);
    }
    public DataItemUnit[] GetUnitInArea(bool attackingSide,  int aX=-1, int aY = -1, int bX=-1, int bY = -1, int cX=-1, int cY = -1 )
    {
        return GetUnitInArea(attackingSide, new Vector2Int[]
        {
            new Vector2Int(aX,aY),
            new Vector2Int(bX,bY),
            new Vector2Int(cX,cY),
        });
    }
    public DataItemUnit[] GetUnitInArea(bool attackingSide, Vector2Int[] position)
    {
        HashSet<DataItemUnit> select = new();
        foreach ( var vector in position)
        {
            if (select != null)
                select.Add(GetUnitAt(attackingSide, vector.x, vector.y));
        }
        return select.ToArray();
    }
    public DataItemUnit[] GetUnitsInRow(bool attackingSide, int row)
    {
        return GetUnitInArea(attackingSide, row, 0, row, 1);
    }
    public DataItemUnit[] GetUnitsInColumn(bool attackingSide, int column)
    {
        return GetUnitInArea(attackingSide, 0, column, 1, column, 2, column);
    }
    public SparseIntMap OutputResults()
    {
        SparseIntMap map = new();
        foreach (var unit in combatants)
        {
            map.Set(unit.eID, (int)unit.damageable.Health.GetValue());
        }
        return map;
    }
}
