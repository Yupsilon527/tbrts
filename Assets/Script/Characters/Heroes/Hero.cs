using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Mob
{
    public HeroClassComponent classComponent;

    public EquipmentComponent equipment;
    public InventoryComponent inventory;
    public HeroVariablesComponent variables;
    protected override void Awake()
    {
        base.Awake();
        if (classComponent == null)
            classComponent = GetComponent<HeroClassComponent>();

        if (equipment == null)
            equipment = GetComponent<EquipmentComponent>();
        if (inventory == null)
            inventory = GetComponent<InventoryComponent>();
        if (variables == null)
            variables = GetComponent<HeroVariablesComponent>();
    }
    public override void Spawn()
    {
        classComponent.SetStats(classComponent.AssignedClass) ;
        base.Spawn();
        abilities.OnSpawn();
        combatant.AutoAssignActiveAbility();
        stats.Recalculate();
    }
    public override void Despawn()
    {
        if (IsPlayerControlled())
        {
            PlayerController.main.heroMan.ForgetHero(this);
        }
        gameObject.SetActive(false);
        base.Despawn();

    }
    #region Selected Attack
    int selectedAttack = 0;

    public string HeroName = "Nameless";

    public void CycleAttack()
    {
        PropertyAbility[] mAttacks = abilities.GetAvailableAttacks(false);
        if (mAttacks.Length > 0)
        {
            selectedAttack = (selectedAttack + 1) % mAttacks.Length;
            combatant.SetActiveAbility(mAttacks[selectedAttack]);
        }
    }
    public PropertyAbility GetSelectedAttack()
    {
        return combatant.GetActiveAbility();
    }
    #endregion
  
}
