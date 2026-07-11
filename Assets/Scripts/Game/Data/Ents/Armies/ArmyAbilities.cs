using System.Collections.Generic;
using System.Linq;

public class BannerAbilities : BannerComponent
{
    public BannerAbilities(DataItemBanner parent) : base(parent)
    {
    }

    public PropertySpell[] GetAllAvaiableSpells()
    {
        HashSet<PropertySpell> spells = new();
        foreach (var u in parent.formation.GetUnits())
        {
            foreach (var spell in u.actions.GetSpells())
            spells.Add(spell);
        }
        return spells.ToArray();
    }
    public PropertySpell[] GetAbilitiesCastable(DataItemTile tile)
    {
        return GetAllAvaiableSpells().Where(s => s!=null && s.CanCastOnTile( tile)).ToArray();
    }
    public bool CastAbilityOnTile(PropertySpell spell, DataItemTile tile)
    {
        CastTable castTable = new(spell.parent, tile.gridPos, spell);
        return spell.CastFromTable(castTable);
    }
}
