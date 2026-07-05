using System.Collections.Generic;
using System.Linq;

public class ArmyAbilities : ArmyComponent
{
    public ArmyAbilities(DataItemArmy parent) : base(parent)
    {
    }

    public PropertySpell[] GetAllAvaiableSpells()
    {
        List<PropertySpell> spells = new();
        foreach (var u in parent.formation.GetUnits())
        {
            spells.AddRange(u.actions.GetSpells());
        }
        return spells.ToArray();
    }
    public PropertySpell[] GetAbilitiesCastable(DataItemTile tile)
    {
        return GetAllAvaiableSpells().Where(s => s!=null && s.CanCastOnTile( tile)).ToArray();
    }
    public bool CastAbilityOnTile(PropertySpell spell, DataItemTile tile)
    {
        return false;
    }
}
