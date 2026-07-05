
using System.Collections.Generic;

public class ArmyAuras : ArmyComponent
{
    public class InnateUnitPair
    {
        public DataItemUnit caster;
        public InnateData innate;

        public InnateUnitPair(DataItemUnit parent, InnateData innate)
        {
            this.caster = parent;
            this.innate = innate;
        }
    }
    public List<InnateUnitPair> auras = new();
    public ArmyAuras(DataItemArmy parent) : base(parent)
    {
    }
    public void OnUnitEnterFormation(DataItemUnit unit)
    {
        foreach (var aura in unit.data.innates)
        {
            auras.Add(new InnateUnitPair(unit,aura));
        }
        foreach (var aura in auras)
        {
            unit.modifiers.ApplyNewModifierFromData(aura.innate,aura.caster);
        }
    }
    public void OnUnitExitFormation(DataItemUnit unit)
    {
        foreach (var aura in unit.modifiers.GetInnates())
        {
            if (!aura.CanApplyToUnit(unit))
            {
                unit.modifiers.Remove(aura);
            }
        }
        auras.RemoveAll(a => a.caster == unit);
    }
}
