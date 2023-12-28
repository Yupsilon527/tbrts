using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttitudeComponent : MobComponent, IRoomTransition
{
    public float WanderTime = 15;
    public float WanderRange = 3;

    public float DefendRange = 3;
    public float AggroRange = -1;
    public enum Attitude
    {
        Passive,    //stands still, retaliates/follows then attacked
        Defensive,  //stands still, attacks closest enemy in aggro range
        Guard, //Like defensive, returns to original pos if idle
        Aggressive, //Aggroes global closest enemy
        Rampant,  //Aggroes random target
        StandGround,//sits still, spams attacks
        Wander,//like passive, wanders when idle
    }
    Attitude currentAttitude;
    public GridNav.Node GuardPoint;
    public void WatchPoint(Attitude nStance, Vector3 UpdatePos)
    {
        GridNav grid = parent.transition.CurrentRoom.grid;
        WatchPoint(nStance, grid.GetClosestToPoint(grid.TranslateCoordinate(UpdatePos)));
    }
    public void WatchPoint(Attitude nStance, GridNav.Node UpdatePos)
    {
        GuardPoint = UpdatePos;
        SetAttitude(nStance);
    }
    public void SetAttitude(Attitude nAttitude)
    {
        currentAttitude = nAttitude;
        nextStanceUpdate = 0;
        parent.combatant.SetAnchored(currentAttitude == Attitude.StandGround);
    }
    public Attitude GetAttitude()
    {
        return currentAttitude;
    }

    bool IsInCombat()
    {
        return parent.combatant.enabled;
    }
    bool IsIdle()
    {
        return parent.orders.NumOrders() == 0;
    }

    float nextStanceUpdate = 0;
    public float HandleStance()
    {
        switch(currentAttitude)
        {
            case Attitude.Wander:
                if (parent.movement != null && WanderRange > 0)
                {
                    Wander();
                    return Random.Range(1, WanderTime);
                }
                break;
            case Attitude.Aggressive:
            case Attitude.Defensive:
            case Attitude.Guard:
            case Attitude.StandGround:
                if (!IsIdle() || IsInCombat())
                {
                    break;
                }
                parent.combatant.SearchNextTarget(GetEffectiveAggroRange()) ;
                if (currentAttitude == Attitude.Guard && !parent.combatant.enabled && GuardPoint!=null)
                {
                    parent.orders.GiveOrder(new OrdersComponent.NodeOrder(GuardPoint), -1);
                }
                return 3f;
            case Attitude.Rampant:
                if (!IsIdle() || IsInCombat())
                {
                    break;
                }
                parent.combatant.AggroRandomTarget();
                return 3f;
            case Attitude.Passive:
                return 100f;
        }
        return 1f;
    }
    public float GetEffectiveAggroRange()
    {
        if (currentAttitude == Attitude.Aggressive)
            return AggroRange;

        if (parent.combatant.GetActiveAbility() != null)
            return Mathf.Max(parent.combatant.GetActiveAbility().GetMaxRange(false) + DefendRange * .5f, DefendRange);

        return DefendRange;
    }

    void Update()
    {
        StanceUpdate();
    }
    public void StanceUpdate()
    {
        if (SanityCheck() && nextStanceUpdate < Time.time)
            nextStanceUpdate = Time.time + HandleStance();
    }
    void Wander()
    {
        if (parent?.transition?.CurrentRoom?.grid != null && parent.movement != null )
        {
            GridNav.Node validTile = parent.transition.CurrentRoom.grid.RandomNodeInCircle(GuardPoint.worldPos, WanderRange);
            if (validTile != null)
            {
                parent.orders.GiveOrder(new OrdersComponent.NodeOrder(validTile),-1);
            }
        }
    }

    public void OnChangeRoom()
    {
        SetAttitude(Attitude.Passive);
    }

}
