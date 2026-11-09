using System.Linq;

public abstract class ProductionSO : ScriptableBase
{
    public ProductionData production;

    public ProductionSO[] prerequisites;

    public void AutoFillPrerequisites(ProductionData prod)
    {
        if (prerequisites!= null && prerequisites.Length > 0) { 
        prod.prerequisites = prerequisites.Where(p => p!=null).Select(u => (u is BuildingSO b ? $"b_"+ b.InternalName : u.InternalName).ToLower()).ToArray();
    }
        else
        {
            prod.prerequisites = new string[0];
        }
    }

}
