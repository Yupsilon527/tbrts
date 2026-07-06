using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Data/Production/Units")]
public class UnitSO : ScriptableObject
{
    public UnitData unit;
    public WeaponSO[] attacks;
    public SpellSo[] spells;
    public CharacterSO character;
    private void OnValidate()
    {
        if (unit != null) unit.InternalName = name;
    }
}
