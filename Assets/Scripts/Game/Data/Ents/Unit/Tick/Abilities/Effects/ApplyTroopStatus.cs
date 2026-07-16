using UnityEngine;

public class ApplyTroopStatus : TroopUniqueEffect
{
    public ModifierDefines.TroopState effect;
    public int turnDuration;
public override void Resolve(DataItemBanner banner)
    {
        banner.status. appliedStatuses.Add(new ArmyStatus(effect, turnDuration + GameManager.main.currentTurn));
    }
}
