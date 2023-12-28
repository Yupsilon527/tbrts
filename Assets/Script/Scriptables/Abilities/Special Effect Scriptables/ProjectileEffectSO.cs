using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Projectile", menuName = "Abilities/Special Effects/On Projectile")]
public class ProjectileEffectSO : SpecialEffectSO
{
    public EmitTime Contact;
    public bool targeted;
    public override void MakeEffect(CastTable castData, float delay)
    {
        GameObject projectile = SpecialEffectPool.main.EffectFromPrefab(EffectPrefab, GetOrigin(castData), delay + GetDelay());
        if (projectile == null)
            return ;
        if (projectile.TryGetComponent(out ProjectileEffectController projeffect))
        {
            if (!targeted)
                projeffect.LaunchTowardsPosition(GetOrigin(castData), castData.GetPointTarget(false), Color.white, delay + GetDelay(), GetTravelTime());
            else if (castData.target != null)
            {
                Transform target = castData.target.transform;
                
                var attach = castData.target.animations.FindAttachPoint(impactPoint); 
                if (attach!= null)
                {
                    target = attach.transform;
                }

                projeffect.LaunchAtTarget(GetOrigin(castData), target, Color.white, delay + GetDelay(), GetTravelTime());
            }
        }
    }
    public string launchOrigin = "launch";
    public string impactPoint = "chest";
    public Vector3 GetOrigin(CastTable castData)
    {
        if (castData.caster != null )
        {
            var attachPoint = castData.caster.animations.FindAttachPoint(launchOrigin);
            if (attachPoint!=null)
            {
                return attachPoint.transform.position;
            }
        }
        return castData.caster.transform.position;
    }
    public float GetTravelTime()
    {
        return ((float)Contact - (float)EmitDelay) * .1f;
    }
}
