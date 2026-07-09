using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Data/Production/Upgrade")]
public class UpgradeSO : ProductionSO
{

    public UpgradeData upgrade;
    public override void OnValidate()
    {
        base.OnValidate();
        if (upgrade != null)
        {
            upgrade.InternalName = InternalName;
            AutoFillPrerequisites(upgrade);
        }
    }
}
