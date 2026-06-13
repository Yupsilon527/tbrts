using UnityEngine;
using VikingParty;

public class InterfaceManager : WindowManager
{
    public static InterfaceManager main;
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
    }

}
