using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Data/Production/Building")]
public class BuildingSO : ProductionSO
{
    public BuildingData building;

    public UnitSO[] armies;

    public override void OnValidate()
    {
        base.OnValidate();
        if (building!=null)
        {
            building.production = armies.Select(u => u == null ? "" : u.unit.InternalName).ToArray();
        }
    }
}
