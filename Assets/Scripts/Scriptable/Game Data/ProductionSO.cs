using System.Linq;
using UnityEngine;

public abstract class ProductionSO : ScriptableBase
{
    public ProductionData production;

    public ProductionSO[] prerequisites;

    public void AutoFillPrerequisites(ProductionData prod)
    {
        prod.prerequisites = prerequisites.Select(u => (u is BuildingSO b ? $"b_"+ b.InternalName : u.InternalName).ToLower()).ToArray();
    }

}
