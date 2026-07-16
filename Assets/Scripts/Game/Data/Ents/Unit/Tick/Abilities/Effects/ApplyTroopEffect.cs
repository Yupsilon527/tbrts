using UnityEngine;

public class ApplyTroopEffect : TroopUniqueEffect
{
    public ModifierDefines.TroopEffect effect;
    public int effectPower;

    public override void Resolve(DataItemBanner banner)
    {
        switch (effect)
        {
            case ModifierDefines.TroopEffect.Revive:
                float percent = Mathf.Clamp01(effectPower * .01f);

                foreach (var u in banner.formation.GetUnits(true, true))
                {
                    if (!u.IsAlive())
                    {
                        u.Revive(percent);
                    }
                }
                break;
            default:
                break;
        }
    }
}
