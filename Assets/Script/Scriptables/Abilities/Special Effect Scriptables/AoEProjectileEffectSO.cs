/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Area Projectiles", menuName = "Abilities/Special Effects/On Area Projectiles")]
public class AoEProjectileEffectSO : SpecialEffectSO
{
    public bool OnCaster = false;
    public int NumProjectiles;
    public override GameObject MakeEffect(CastTable castData, float emitdelay, float contactdelay)
    {
        Vector3 center = OnCaster ? castData.caster.GetWorldPosition() : stateInGame.main.mapData.TranslateGridPosition(castData.point);
        float AoERange = castData.ability.GetAreaRange();

        for (int Zim = 0; Zim < NumProjectiles; Zim++)
        {
            float angle = Zim * (360f / NumProjectiles);


            GameObject projectile = SpecialEffectPool.main.PoolObject(EffectPrefab);
            if (projectile == null)
                return null;
            if (projectile.TryGetComponent(out ProjectileEffectController projeffect))
            {
                Vector3 destination = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * AoERange;
                projeffect.LaunchTowardsPosition(castData.caster.GetPowerColor(), contactdelay, contactdelay - emitdelay, center, destination);
            }
        }
        return null;
    }

}
*/