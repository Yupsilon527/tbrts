using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Data/Production/Upgrade")]
public class UpgradeSO : ProductionSO
{
    public ActionSO[] actions;
    public ModifierPassive[] innates;
    public UpgradeData upgrade;
    public override void OnValidate()
    {
        base.OnValidate();
        if (upgrade != null)
        {
            upgrade.InternalName = InternalName;
            AutoFillPrerequisites(upgrade);
            upgrade.tempSpells = actions.Select(a => a.Translate()).ToArray();
            upgrade.innatesAdded = innates.Select(a => a.Translate() as InnateData).ToArray();
        }
    }
}
