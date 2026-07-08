using UnityEngine;

public class PlayerEconomy : PlayerComponent
{
    public Resource[] resources;
    public float[] baseIncome = new float[(int)EconomyDefines.IncomeResource.Total];
    public float[] realIncome = new float[(int)EconomyDefines.IncomeResource.Total];

    public PlayerEconomy(DataItemPlayer player) : base(player)
    {
        resources = new[] {
            new ResourceInt(0, "Player Metal", false, false),
            new ResourceInt(0, "Player Gold", false, false),
            new ResourceInt(100, "Player Mana", false, true)
    };
    }
    public Resource GetResource(EconomyDefines.EconomyResource res)
    {
        if ((int)res < resources.Length)
            return resources[(int)res];
        return new ResourceInt(0, "temp", false, false);  //TODO
    }
    public float GetResourceValue(EconomyDefines.EconomyResource resource)
    {
        if ((int)resource < resources.Length)
            return GetResource(resource).GetValue();
        return 0;
    }
    public bool CanAffordResource(ResourceCost check, float mult = 1)
    {
        if (check.value <= 0 || check.resource == EconomyDefines.EconomyResource.Labor) return true;
        return GetResourceValue(check.resource) >= check.value * mult;
    }
    public bool CanAffordResources(ResourceCost[] resourceCosts, float mult = 1)
    {
        foreach (var cost in resourceCosts)
        {
            if (!CanAffordResource(cost, mult))
            {
                return false;
            }
        }
        return true;
    }
    public void GiveResources(ResourceCost[] granted, float mult = 1, bool roundstart = false)
    {
        foreach (var res in granted)
            GiveResource(res, mult, roundstart);
    }
    public void GiveResource(ResourceCost granted, float mult = 1, bool roundstart = false)
    {
        GiveResource(granted.resource, granted.value * mult, roundstart);
    }
    public void GiveResource(EconomyDefines.EconomyResource resource, float value, bool roundstart = false)
    {
        if ((int)resource < resources.Length)
            GetResource(resource).GiveValue(value);
    }
    public float Spend(ResourceCost spent)
    {

        if ((int)spent.resource < resources.Length)
            return GetResource(spent.resource).SubstractedValue(spent.value);
        return 0;
    }
    public void SpendResources(ResourceCost[] spent)
    {
        foreach (var res in spent)
        {
            Spend(res);
        }
    }
    public void RefundCosts(ResourceCost[] refund)
    {
        foreach (var r in refund)
            Refund(r);
    }
    public void Refund(ResourceCost refund)
    {
        GetResource(refund.resource).GiveValue(refund.value);
    }
    public void RegisterIncome(ResourceIncome[] income, float mult)
    {
        foreach (var e in income)
            IncreaseIncome(e, mult);
        if (income.Length > 0)
            ReviseRealIncome();
    }
    public void DeregisterIncome(ResourceIncome[] income, float mult)
    {
        foreach (var e in income)
            IncreaseIncome(e, 0 - mult);
    }
    #region Income
    public void IncreaseIncome(ResourceIncome income, float mult)
    {
        IncreaseIncome(income.resource, income.value * mult);

    }
    public void IncreaseIncome(EconomyDefines.IncomeResource resource, float delta)
    {
        baseIncome[(int)resource] += delta;
    }
    public void DecreaseIncome(EconomyDefines.IncomeResource resource, float delta)
    {
        baseIncome[(int)resource] = Mathf.Max(0, baseIncome[(int)resource] - delta);
    }
    public void HandleIncome(float minute)
    {
        for (int i = 0; i < (int)EconomyDefines.EconomyResource.Total; i++)
        {
            GiveResource((EconomyDefines.EconomyResource)i, GetIncome((EconomyDefines.EconomyResource)i, minute));
        }
    }
    float GetIncome(EconomyDefines.EconomyResource resource, float minute)
    {
        return realIncome[(int)resource] * (1 + realIncome[(int)resource + (int)EconomyDefines.IncomeResource.RawIncome]) * minute;
    }
    public void ReviseRealIncome()
    {
        /*  if (LevelController.main.gameState != LevelController.GameState.playing) return;
          for (int i = 0; i < baseIncome.Length; i++)
          {
              realIncome[i] = baseIncome[i];
              foreach (var b in buildings)
              {
                  if ((EconomyDefines.IncomeResource)i == EconomyDefines.IncomeResource.PopLimit)
                      realIncome[i] += b.buildingData.GetResourceIncome((EconomyDefines.IncomeResource)i);
                  else
                      realIncome[i] += b.buildingData.GetResourceIncome((EconomyDefines.IncomeResource)i) * b.stats.realStats.HarvestRate;
              }
          }

      */
    }
    public void HandleIncome()
    {
        GiveResource(EconomyDefines.EconomyResource.Metal, realIncome[(int)EconomyDefines.IncomeResource.Metal]);
        GiveResource(EconomyDefines.EconomyResource.Gold, realIncome[(int)EconomyDefines.IncomeResource.Gold]);
        HandleManaIncome();
    }
    void HandleManaIncome()
    {
        var manaResource = GetResource(EconomyDefines.EconomyResource.Mana);

        float manaIncome = realIncome[(int)EconomyDefines.IncomeResource.Mana];
        float manaIncomeMin = realIncome[(int)EconomyDefines.IncomeResource.ManaMin];
        float manaIncomeMax = realIncome[(int)EconomyDefines.IncomeResource.ManaMax];

        float mana = Mathf.Min(manaIncomeMin - manaResource.GetValue(), manaIncome);
        manaResource.SetLimit(manaIncomeMax);
        GiveResource(EconomyDefines.EconomyResource.Mana, mana);
    }
    #endregion
}
