using UnityEngine;

public class ScriptableBase : ScriptableObject
{
    public string InternalName;
    public string AssignedWorld;
    public virtual void OnValidate()
    {
        if (InternalName == "" || InternalName == "MISSING")
            InternalName = name;
    }
}
