using System.Collections.Generic;
using UnityEngine;

public class CityProduction : CityComponent
{
    public float iProductionTime = 0;
    public bool ContinuousProduction = false;

    public List<UnitData> availableUnits = new List<UnitData>();

    public List<ProductionTable> productionQueue = new List<ProductionTable>();
    public List<Vector2Int> wayPoints = new List<Vector2Int>();

    public CityProduction(DataItemCastle city) : base(city)
    {
    }

    public int GetMyProdLimit()
    {
        return 5;
    }
    public override void Revision()
    {
        base.Revision();
        availableUnits.Clear();
        foreach (var unit in city.bonuses.upgrades.researchedUpgrades)
        {
            if (unit.upgrade is BuildingData building)
            {
                foreach (var unitID in building.production)
                {
                    availableUnits.Add(WorldManager.main.LoadUnit(unitID));
                }
            }
        }
    }

    #region Production

    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        ForwardProduction();
    }
    public float GetFirstItemLaborCost()
    {
        if (productionQueue.Count>0)
            return productionQueue[0].costs[(int)EconomyDefines.EconomyResource.Labor].value;
        return 0;
    }
    void ForwardProduction()
    {

        if (CanProduce())
        {
            iProductionTime += city.income.baseIncome[(int)EconomyDefines.IncomeResource.Labor];
            while (productionQueue.Count > 0)
            {

                var prod = productionQueue[0];
                var costLabor = prod.costs[(int)EconomyDefines.EconomyResource.Labor].value;

                if (iProductionTime >= costLabor)
                {
                    if (productionQueue[0].CompleteProduction())
                    {
                        RemoveProduction(0, false);
                        if (ContinuousProduction)
                        {
                            AddProduction(prod.production, false);
                        }
                    }
                    else return;
                }
                else break;
            }
            if (productionQueue.Count == 0)
                iProductionTime = 0;
        }
        else
        {
            iProductionTime = 0;
        }
    }
    public bool CanProduce()
    {
        return !city.IsDemolished() && city.AmIUnderAlliedControl();
    }
    public bool CanProduce(ProductionData prod)
    {
        if (prod is UnitData unit)
            return GetValidTileForArmy(unit) != null;
        else if (prod is BuildingData building)
            return !city.bonuses.HasBuilding(building.InternalName);
        return true;
    }
    public void AddProduction(ProductionData p, bool instant)
    {
        if (p.GetAvailableState(city) == ProductionData.AvailableState.available  && CanProduce(p))
        {
            var prodTable = new ProductionTable(city.GetPlayerOwner(), p, city);

            city.GetPlayerOwner().econ.SpendResources(prodTable.costs);

            if (instant || prodTable.costs[(int)EconomyDefines.EconomyResource.Labor].value == 0)
            {

                prodTable.CompleteProduction();
            }
            else
            {
                productionQueue.Add(prodTable);
            }
        }
    }
    void RemoveProduction(int id, bool refund)
    {
        RemoveProduction(productionQueue[id],refund);
    }
    void RemoveProduction(ProductionTable table, bool refund)
    {
        if (refund)
            city.GetPlayerOwner().econ.RefundCosts(table.costs);
        productionQueue.Remove(table);
    }


    public SidewaysTile GetValidTileForArmy(UnitData Army)
    {
        var movement = Army.GetMovetype();

        List<SidewaysTile> passibleTiles = new();
        if (TerrainDefines.CanIWalkOver(movement, TerrainDefines.Elevation.City))
        {
            passibleTiles.AddRange(city.castleTiles);
        }
        else
        {
            foreach (var tile in city.castleTiles)
            {
                foreach (var n in tile.neighbors)
                {
                    if (n.IsPassible(movement))
                        passibleTiles.Add(n);
                }
            }
        }
        passibleTiles.RemoveAll(t => t.armyLayer != null
        && (t.armyLayer.GetAlignment(city.GetPlayerOwner()) != PlayerDefines.Alignment.playerowned
        || !t.armyLayer.formation.CanIAccept(Army))
        );

        if (passibleTiles.Count > 0 ) { return passibleTiles[0]; }


        return null;

    }

    #endregion
    public override void OnCastleRaze()
    {
        base.OnCastleRaze();
        productionQueue.Clear();
        wayPoints.Clear();
        Revision();
    }
}
