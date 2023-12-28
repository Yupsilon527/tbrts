using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersComponent : MobComponent
{
    public class MoveOrder
    {
        public Vector2 pos;
        protected float range = 0;

        public MoveOrder(Vector2 pos, float approachRange = 0) 
        {
            this.pos = pos;
            range = approachRange;
        }

        public virtual void Resolve(Mob solver)
        {
            solver.Cancel();
            solver.movement.Move(pos);
        }
        public virtual bool HasResolvedOrder(Mob solver)
        {
            if (range > 0)
            return ((Vector2)solver.transform.position - pos).sqrMagnitude <= range * range;
            else
                return solver.pathfinder.progress != PathfinderComponent.Failure.incomplete;
        }
        public virtual void Conclude(Mob solver)
        {
            solver.movement.Stop(false) ;
        }
    }
    public class NodeOrder: MoveOrder
    {
        GridNav.Node node;

        public NodeOrder(GridNav.Node node, float approachRange = 0) : base(node.worldPos, approachRange)
        {
            this.node = node;
        }

        public override void Resolve(Mob solver)
        {
            solver.Cancel();
            solver.movement.Move(node);
        }
    }
    public class AttackOrder : MoveOrder
    {
        public Mob attackTarget;

        public AttackOrder(Mob interactObject) : base(interactObject.transform.position)
        {
            attackTarget = interactObject;
        }

        public override void Resolve(Mob solver)
        {
            if (!solver.combatant.enabled)
            {
                solver.Cancel();
            }
            if ( solver.combatant.attackTarget != attackTarget)
            {
                    solver.combatant.EngageCombat(attackTarget);
                
            }
        }
        public override bool HasResolvedOrder(Mob solver)
        {
            if (!solver.combatant.enabled)
            {
                    return true;
            }
            return false;
        }
    }
    public class AttackMoveOrder : MoveOrder//TODO
    {

        public AttackMoveOrder(Vector2 pos) : base(pos)
        {
            this.pos = pos;
        }

        /*public override void Resolve(Mob solver)
        {
            solver.movement.Move(pos);
        }
        public override bool HasReachedTarget(Mob solver)
        {
            solver.attitude.StanceUpdate();
            return solver.pathfinder.progress != PathfinderComponent.Failure.incomplete;
        }*/
    }
    public class AttackGroupOrder : MoveOrder
    {
        public Mob[] targets;

        public AttackGroupOrder(Mob[] targets) : base(Vector2.zero)
        {
            this.targets = targets;
        }
        public override bool HasResolvedOrder(Mob solver)
        {
            for (int I =0; I< targets.Length; I++)
            {
                if (targets[I]==null)
                {
                    continue;
                    
                }
                else if (targets[I].damageable.isAlive())
                {
                    if (solver.combatant.CanFightTarget(targets[I]))
                    solver.orders.GiveOrder(new AttackOrder(targets[I]), -1);
                    return false;
                }
                else
                {
                    targets[I] = null;
                    continue;
                }
            }
            return true;
        }
    }
    public class CastOrder : MoveOrder
    {
        Mob attackTarget;
        PropertyAbility ability;

        public CastOrder(Mob interactObject, PropertyAbility ability) : base(interactObject.transform.position)
        {
            attackTarget = interactObject;
            this.ability = ability;
        }
        public override void Resolve(Mob solver)
        {
            solver.Cancel();
        }
        public override bool HasResolvedOrder(Mob solver)
        {
            if (ability == null || !ability.IsFullyCastable() || solver.abilities == null  || ability.CanCastOnTarget(attackTarget))
            {
                return true;
            }
            else if (!solver.movement.IsWalking())
            {
                solver.movement.Follow(attackTarget.gameObject);
            }
            return false;
        }
        public override void Conclude(Mob solver)
        {
            if (ability.CanCastOnTarget(attackTarget))
            {
                solver.Cancel();
                CastTable cast = solver.abilities.CastAbilityOnTarget(ability, attackTarget);
                solver.abilities.ResolveCastData(cast);
            }
        }
    }
        List<MoveOrder> orders = new List<MoveOrder>();
    public MoveOrder GetCurrentOrder()
    {
        if (tempOrder != null) return tempOrder;
        if (orders.Count == 0) return null;
        return orders[0];
    }
    public int NumOrders()
    {
        return orders.Count;
    }
    public void GiveOrder(MoveOrder o, int index)
    {
        if (!SanityCheck()) return;
        if (index >= orders.Count)
        {
            orders.Add(o);
        }
        else
        {
            orders.Insert(Mathf.Max(index, 0), o);
        }
        if (orders.Count == 1 || index < 1)
            ResolveCurrentOrder();
    }
    public void ReplaceOrder(MoveOrder o)
    {
        ClearOrders();
        GiveOrder(o,0);
    }
    public void AdvanceOrder()
    {
        if (tempOrder==null && orders.Count > 0)
        {
            orders.RemoveAt(0);
            ResolveCurrentOrder();
        }
    }
    public void ClearOrders()
    {
        orders.Clear();
    }
    void ResolveCurrentOrder()
    {
        if (tempOrder != null)
        {
            tempOrder.Resolve(parent);
        }
        else if (orders.Count > 0)
        {
            GetCurrentOrder().Resolve(parent);
        }
    }

    public void StoreLastOrder()
    {
        if (orders.Count == 0)
        {
            /*if (parent.combatant != null && parent.combatant.attackTarget != null)
            {
                GiveOrder(new AttackOrder(parent.combatant.attackTarget),999);
            }
            else*/
            {
                GiveOrder(new MoveOrder(transform.position), 999);
            }
        }
    }
    MoveOrder tempOrder;
    public void GiveTempOrder(MoveOrder order)
    {
        if (!SanityCheck()) return;
        StoreLastOrder();
        tempOrder = order;
        ResolveCurrentOrder();
    }
    public bool HasTempOrder()
    {
       return tempOrder != null;
    }
    public void ClearTempOrder()
    {
        tempOrder = null;
        ResolveCurrentOrder();
    }
    private void Update()
    {
        if (SanityCheck() &&  OrderPass())
        {
            AdvanceOrder();
        }
        
    }
    bool OrderPass()
    {
            var currentOrder = GetCurrentOrder();
            if (currentOrder != null && currentOrder.HasResolvedOrder(parent))
        {
            currentOrder.Conclude(parent);
            return true;
            
        }
        return false;
    }

    internal bool IsIdle()
    {
        return !HasTempOrder() && GetCurrentOrder() == null;
    }
}
