using UnityEngine;

public class TamperData : ApplyEffects
{
    public AttackDefines.ReactionType attack;

    public override bool Resolve(CastTable table, float strength = 1)
    {
      
        return base.Resolve(table, strength);
    }
    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        throw new System.NotImplementedException();
    }
}
