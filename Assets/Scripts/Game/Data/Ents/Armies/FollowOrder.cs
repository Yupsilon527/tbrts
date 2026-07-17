using UnityEngine;

public class FollowOrder : Order
{
    public DataItemBanner TargetUnit;

    public FollowOrder(ID orderID, Vector2Int gridDest, DataItemBanner targetUnit) : base(orderID, gridDest)
    {
        TargetUnit = targetUnit;
    }

    public virtual bool TargetValid(DataItemBanner owner)
    {
        if (TargetUnit != null && TargetUnit.IsAlive())
        {
            if (!TargetUnit.IsVisibleToAnother(owner))
                return false;
            return true;
        }
        return false;
    }
    public override void RecalcPath(DataItemBanner owner, Vector2Int origin)
    {
        if (TargetValid(owner))
        {
            gridDest = TargetUnit.GetCoords();
            base.RecalcPath(owner, origin);
        }
    }
    public override bool HasResolvedOrder(DataItemBanner owner)
    {
        return !TargetValid(owner) || base.HasResolvedOrder(owner);
    }
}
