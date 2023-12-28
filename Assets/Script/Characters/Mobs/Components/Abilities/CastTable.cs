using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CastTable
{
    public Mob caster;
    public PropertyAbility ability;
    public Vector2 origin;
    public Vector2 point;
    public Mob target;
    //public TileRayScan.Result rayresult;

    public CastTable(Mob caster = null, PropertyAbility ability = null, Mob mob = null, Mob[] predefinedTargets = null)
    {
        this.ability = ability;

        if (ability != null && caster == null)
            caster = ability.caster;
        else 
            this.caster = caster;

        if (caster != null)
        {
            origin = caster.transform.position;
            point = caster.transform.position;
        }
        else
            Debug.LogError("ERROR! Castdata with null caster provided");

        if (target !=null)
        {
            if (ability.GetCastBehavior() != AbilityDefines.Behavior.point)
            target = mob;
            point = mob.transform.position;
        }

        hits = predefinedTargets;
    }
    public void UpdatePointTarget(Mob mob)
    {
         target = mob;
        UpdatePointTarget(target.transform.position);
    }
    public void UpdatePointTarget(Vector2 targetPoint)
    {

        point = targetPoint;
        UpdateHits(true);
    }
    public Vector2 GetPointTarget(bool real)
    {
        if (!real )
        {
         if (target!=null)
        {
            return target.transform.position;
        }
        }
        
            return point;
    }
    public bool CanCastOnPoint(Vector2 tile)
    {
        return ability.CanCastOnPoint(tile);
    }
    Mob[] hits;
    public void UpdateHits(bool fresh)
    {
        if (ability == null) return;
        List<Mob> entities = new List<Mob>();
        foreach (Mob entity in ability.CheckHitMobs( this))
        {
            if (!entities.Contains(entity) && ability.IsValidTarget(entity))
            {
                entities.Add(entity);
            }
        }
        if (ability.AbilityBehavior == AbilityDefines.Behavior.target && target != null && !entities.Contains(target) && ability.IsValidTarget(target))
        {
            entities.Add(target);
        }
        hits = entities.ToArray();
    }
    public void SetHits(Mob target)
    {
        hits = new Mob[] { target };
    }
    public void UpdateHits(Mob[] SetHits)
    {
        hits = SetHits;
    }
    public Mob[] GetHitEntities(bool fresh)
    {
        if (fresh || hits == null) UpdateHits(fresh);
        return hits;
    }
}