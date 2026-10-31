public abstract class TroopUniqueEffect : ApplyEffects
{
    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        table.target.troop.status.ApplyStatus(this);
    }
    public abstract void Resolve(DataItemBanner banner);
}
