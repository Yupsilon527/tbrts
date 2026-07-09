using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Data/Production/Units")]
public class UnitSO : ProductionSO
{
    public UnitData unit;
    public WeaponSO[] attacks;
    public SpellSo[] spells;
    public ModifierPassive[] passives;
    public CharacterSO character;

    public override void OnValidate()
    {
        if (unit != null)
        {
            unit.InternalName = name;
            AutoFillPrerequisites(unit);
        }
    }
}
