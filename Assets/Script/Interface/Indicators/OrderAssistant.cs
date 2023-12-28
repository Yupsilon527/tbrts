using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAssistant : MonoBehaviour
{
    public static OrderAssistant main;
    private void Awake()
    {
        main = this;
        enabled = false;
    }
    [Header("Movement Indicator")]
    public GameObject IndicatorPrefab;
    List<MovementIndicator> indicatorList = new List<MovementIndicator>();

    public void RegisterSingleMob(Mob single)
    {
        PoolIndicatorForHero(single);
    }
    public void RegisterHeroes(Mob startingHero)
    {
        if (startingHero!=null)
            PoolIndicatorForHero(startingHero);

        foreach (Mob hero in PlayerController.main.party.heroes)
        {
            if (hero.IsSelected() && startingHero!=hero)
            {
                PoolIndicatorForHero(hero);
            }
        }
    }
    MovementIndicator PoolIndicatorForHero(Mob hero)
    {
        GameObject indicator = SpecialEffectPool.main.PoolItem(IndicatorPrefab);
        indicator.SetActive(true);
        if (indicator.TryGetComponent(out MovementIndicator indi))
        {
            indi.SetOwner(hero);
            indicatorList.Add(indi);
            return indi;
        }
        return null;
    }


    public void OnDisable()
    {
        ClearIndicators();
    }
    void ClearIndicators()
    {
        foreach (MovementIndicator mi in indicatorList)
        {
            SpecialEffectPool.main.DeactivateObject(mi.gameObject);
        }
        indicatorList.Clear();
    }

    #region Move
   Vector2 origin;
    public void HighlightMob(Mob target)
    {
        if (indicatorList.Count > 0)
        {
            foreach (MovementIndicator mi in indicatorList)
            {
                mi.destination = target.transform.position;
               // mi.gameObject.SetActive(true);
            }
        }
    }

    public void HighlightPoint(Vector2 target)
    {
        float CohesionDistanceThreshold = 2;//todo define
        float FormationDistance = 3;//todo define
        if (indicatorList.Count > 0)
        {
            origin = indicatorList[0].origin;
            foreach (MovementIndicator mi in indicatorList)
            {
                Vector2 delta = ((Vector2)mi.origin - origin);

                if (delta.sqrMagnitude > CohesionDistanceThreshold * CohesionDistanceThreshold)
                {
                    delta = delta.normalized * FormationDistance * ((CohesionDistanceThreshold * CohesionDistanceThreshold) / delta.sqrMagnitude);
                }
                mi.destination = target + delta;
            }
        }
    }

    #endregion

    #region Order Indicator
    [Header("Orders")]
    public GameObject MoveIndicatorPrefab;
    public GameObject TargetAllyIndicatorPrefab;
    public GameObject TargetEnemyIndicatorPrefab;
    public void DrawIndicatorForOrder(OrdersComponent.MoveOrder order)
    {
        if (SpecialEffectPool.main == null) return;

        if (order is OrdersComponent.AttackGroupOrder groupOrder)
        {
            foreach (Mob target in groupOrder.targets)
            {
                DoAttackIndicator(target.transform.position);
            }
        }
        else if (order is OrdersComponent.AttackOrder attackOrder)
        {
            if (attackOrder.attackTarget.IsPlayerControlled())
            {
                DoAllyIndicator(attackOrder.attackTarget.transform.position);
            }
            else
            {
                DoAttackIndicator(attackOrder.attackTarget.transform.position);
            }
        }
        else
        {
            DoMoveIndicator(order.pos);
        }
    }

    void DoMoveIndicator(Vector2 effectPosition)
    {
        if (MoveIndicatorPrefab != null && SpecialEffectPool.main != null)
        {
            SpecialEffectPool.main.EffectFromPrefab(MoveIndicatorPrefab, effectPosition);
        }
    }
    void DoAllyIndicator(Vector2 effectPosition)
    {
        if (TargetAllyIndicatorPrefab != null && SpecialEffectPool.main != null)
        {
            SpecialEffectPool.main.EffectFromPrefab(TargetAllyIndicatorPrefab, effectPosition);
        }
    }
    void DoAttackIndicator(Vector2 effectPosition)
    {
        if (TargetEnemyIndicatorPrefab != null && SpecialEffectPool.main != null)
        {
            SpecialEffectPool.main.EffectFromPrefab(TargetEnemyIndicatorPrefab, effectPosition);
        }
    }
    #endregion
}
