using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInterfaceController : InterfaceParent
{
    public static DungeonInterfaceController main;
    public AbilityInterface abilities;
    public HeroSelectionController portraits;
    public ManaIndicator manacrystal;

    List<Mob> selection = new List<Mob>();

    protected virtual void Awake()
    {
        if (abilities == null)
            abilities = GetComponentInChildren<AbilityInterface>();
        if (portraits == null)
            portraits = GetComponentInChildren<HeroSelectionController>();
        if (manacrystal == null)
            manacrystal = GetComponentInChildren<ManaIndicator>();

        main = this;
        ClearSelection();
    }
    private void Start()
    {
        if (PlayerController.main.resources!=null)
        {
            manacrystal.UpdateValue(PlayerController.main.resources.mana);
            PlayerController.main.resources.mana.OnValueChanged.AddListener(() =>
            {
                manacrystal.UpdateValue(PlayerController.main.resources.mana);
            });
        }
    }
    //TODO selection component
    public override void SelectHero(Mob player)
    {
        selection.Add(player);
        OnSelectionChanged();
    }
    public override void DeselectHero(Mob player)
    {
        selection.Remove(player);
        OnSelectionChanged();
    }
    public override void ClearSelection()
    {
        selection.Clear();
        OnSelectionChanged();
    }
    public override void OnSelectionChanged()
    {

        /*if (abilities != null)
        {
            abilities.ClearAbilities();
            if (selection.Count > 0)
            {
                abilities.UpdateAbilityButtons(selection);
                gameObject.SetActive(true);
            }
            else
            {
                abilities.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }*/
        base.OnSelectionChanged();
    }
    public override void OnStuffChanged()
    {
        if (abilities != null)
            abilities.Refresh();
        base.OnStuffChanged();
    }
}
