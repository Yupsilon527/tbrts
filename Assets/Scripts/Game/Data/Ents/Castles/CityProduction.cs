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
                availableUnits.AddRange(building.production);
            }
        }
    }

    #region Production

    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        ForwardProduction();
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

                if (iProductionTime > costLabor)
                {
                    productionQueue[0].CompleteProduction();
                    RemoveProduction(0,false);
                    if (ContinuousProduction)
                    {
                        AddProduction(prod.production,false);
                    }
                }
            }
        }
    }
    public bool CanProduce()
    {
        return !city.isRazed() && city.AmIUnderAlliedControl();
    }
    public bool CanProduce(ProductionData prod)
    {
        if (prod.GetAvailableState(city) != ProductionData.AvailableState.available) return false;
        if (prod is UnitData unit)
            return GetValidTileForArmy(unit) != null;
        else if (prod is BuildingData building)
            return !city.bonuses.HasBuilding(building.InternalName);
        return true;
    }
    public void AddProduction(ProductionData p, bool instant)
    {
        if (CanProduce(p))
        {
            var prodTable = new ProductionTable(city.GetPlayerOwner(), p, city);

            if (!city.GetPlayerOwner().econ.CanAffordResources(prodTable.costs)) return;

            city.GetPlayerOwner().econ.SpendResources(prodTable.costs);

            if (instant)
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
