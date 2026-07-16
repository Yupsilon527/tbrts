using System;

[Serializable]
public class GrantResources : TroopUniqueEffect
{
    public ResourceCost[] resourceAmount;

    public override void Resolve(DataItemBanner banner)
    {
        banner.GetPlayerOwner().econ.GiveResources(resourceAmount);
    }
}
