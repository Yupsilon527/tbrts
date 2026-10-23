using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Properties", menuName = "Abilities/Effects/Modifiers/Properties")]
public class AlterationSO : TagSO
{
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.Flag flag = ModifierDefines.Flag.Tag;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
    public AbilitySave[] abilities = new AbilitySave[0];
    public override TagData Translate()
    {
        return new InheritAbilityData(
            InternalName,
            sprite,
            uibehavior,
            priority,
            flag,
            properties,
            states,
            abilities.Select((a)=> new AbilityData(a.abilityID.ToString(), a.abilityLevel)).ToArray()
            );;
    }
}



[Serializable]
public class AbilitySave
{
    public UnitDefines.ArmyAbilities abilityID;
    public int abilityLevel;
}
