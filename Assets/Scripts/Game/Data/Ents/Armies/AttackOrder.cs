using Astar;
using UnityEngine;
public class AttackOrder : FollowOrder
{
    public AttackOrder(ID orderID, Vector2Int gridDest, DataItemBanner targetUnit) : base(orderID, gridDest, targetUnit)
    {
    }

    public override bool Resolve(DataItemBanner owner)
    {
        if (TargetValid(owner))
        {
            if (owner.BattleAnother(TargetUnit))
                return false;
            /* SparseIntMap results = Combat.main.MockBattle(owner, TargetUnit, TargetUnit.tile);

             var allUnits = new List<DataItemUnit>();
             allUnits.AddRange(owner.formation.GetUnits());
             allUnits.AddRange(TargetUnit.formation.GetUnits());


             foreach (var result in results._entries)
             {
                 var unit = allUnits.FirstOrDefault(u => u.eID == result.key);
                 Combat.main.Inspect($"Unit {unit} remaining with {result.value} health!");
             }
            */
        }
        return true;
    }

}