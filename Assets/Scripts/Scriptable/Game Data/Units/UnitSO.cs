using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Data/Production/Units")]
public class UnitSO : ProductionSO
{
    public UnitData unit;
    public WeaponSO[] attacks;
    public SpellSO[] spells;
    public ModifierPassive[] passives;
    public CharacterSO character;

    public override void OnValidate()
    {
        if (unit != null)
        {
            unit.InternalName = name;
            try { 
            AutoFillPrerequisites(unit);
        }
            catch (Exception e)
            {
                Debug.LogError(unit + " Prereq Error: " + e.Message);   
            }
    }
    }
}
