using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BannerStatus : BannerComponent
{
    public HashSet<TroopUniqueEffect> pendingStatuses = new();
    public HashSet<ArmyStatus> appliedStatuses = new();

    public BannerStatus(DataItemBanner parent) : base(parent)
    {
    }

    public void ApplyPendingStatus(TroopUniqueEffect status)
    {
        if (!pendingStatuses.Contains(status))
            pendingStatuses.Add(status);
    }
    public void ResolvePendingStatuses()
    {
        foreach (var effect in pendingStatuses)
        {
            effect.Resolve(parent);
        }
        ClearPendingStatuses();
    }
    public void ClearPendingStatuses()
    {
        pendingStatuses.Clear();
    }
    public override void OnTurnBegin()
    {
        appliedStatuses.RemoveWhere(s => s.turnExpire <= GameManager.main.currentTurn);
        base.OnTurnBegin();
    }
    public bool HasState(ModifierDefines.TroopState state)
    {
        return appliedStatuses.Any(e => e.state == state);
    }
    public void RemoveState(ModifierDefines.TroopState state)
    {
        appliedStatuses.RemoveWhere(e => e.state == state);
    }
}

public class ArmyStatus
{
    public ModifierDefines.TroopState state;
    public int turnExpire;

    public ArmyStatus(ModifierDefines.TroopState states, int turnExpire)
    {
        this.state = states;
        this.turnExpire = turnExpire;
    }
}
