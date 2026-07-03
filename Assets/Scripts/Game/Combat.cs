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
        enabled = false;
    }

    public SparseIntMap MockBattle(DataItemArmy a, DataItemArmy d, SidewaysTile l)
    {
        mockBattle = true;
        SetUp(a, d, l);
        ResolveInstantly();
        return OutputResults();
    }
    public void BattleTroops(DataItemArmy a, DataItemArmy d, SidewaysTile l)
    {
        mockBattle = false;
        SetUp(a, d, l);
        ResolveInstantly();
    }
    bool mockBattle = false;

    public CombatDefines.AttackPhase currentPhase;
    public SidewaysTile locatedTile;
    public DataItemArmy attackers, defenders;

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
        foreach (var c in combatants)
        {
            Inspect($"{c} at {c.health.GetValue()} health");
        }
    }
    void BeginCombat()
    {
        Inspect($"COMBAT - Begin Combat!");

        enabled = true;
        currentPhase = CombatDefines.AttackPhase.BeforeCombat;
        currentTick = 0;
        FireEventOnAllFighters(AbilityDefines.Event.CombatBegin);
        currentPhase = CombatDefines.AttackPhase.Prep;
        FireEventOnAllFighters(AbilityDefines.Event.CombatPhase);
    }
    public bool IsInCombat()
    {
        return enabled;
    }
    void ResolveInstantly()
    {
        BeginCombat();
        int soft = 0;
        while (enabled && ++soft < 1000)
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

        foreach (var c in combatants)
        {
            if (c.actions.GetAttacks().Any(w => w.CanBeCast(currentPhase)))
            {
                Inspect($"{c} acts at tick {c.nextAction}/{currentPhase}!");
                currentTick = c.nextAction;
                c.Act();
                return;
            }
        }
            ForwardPhase();

    }
    void ForwardPhase()
    {
        if (currentPhase == CombatDefines.AttackPhase.PostAttack)
        {
            EndCombat();
        }
        else
        {
            currentPhase++;
            FireEventOnAllFighters(AbilityDefines.Event.CombatPhase);
        }
    }
    void EndCombat()
    {
        int lastTick = 0;

        foreach (var c in combatants)
        {
            lastTick = Mathf.Max(lastTick, c.nextAction);
        }
        foreach (var c in combatants)
        {
            c.Act(lastTick);
        }
        currentPhase = CombatDefines.AttackPhase.AfterCombat;
        if (!mockBattle)
        {
            foreach (var unit in combatants)
            {
                unit.health.SetValue(unit.damageable.Health.GetValue());
            }
            attackers.PostDamageUpdate();
            defenders.PostDamageUpdate();
        }

        if (enabled)
        {
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
    public DataItemArmy GetOppositeSide(DataItemArmy troop)
    {
        if (attackers.GetAlignment(troop) == PlayerDefines.Alignment.enemy)
        return attackers;
        return defenders;
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
    public DataItemUnit[] GetUnitInArea(bool attackingSide, int aX = -1, int aY = -1, int bX = -1, int bY = -1, int cX = -1, int cY = -1)
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
        foreach (var vector in position)
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
        SparseIntMap map = new(combatants.Count);
        foreach (var unit in combatants)
        {
            map.Set(unit.eID, (int)(unit.damageable?.Health?.GetValue() ?? 0));
        }
        return map;
    }
}
