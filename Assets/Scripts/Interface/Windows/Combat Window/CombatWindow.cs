using UnityEngine;

public class CombatWindow : Window
{
    public CombatantContainer[] containers;
    DataItemBanner attacker, defender;
    public CombatSide attackers, defenders;
    public GameObject buttonsParent;
    private void Start()
    {
        containers = GetComponentsInChildren<CombatantContainer>(true);
    }
    protected override void OnClosed()
    {
        attackers.Clear(); defenders.Clear();
        base.OnClosed();
    }
    public void PresentSides(DataItemBanner a, DataItemBanner d)
    {
        attacker = a;
        defender = d;
        Refresh();
    }
     void LoadArmy(DataItemBanner a, bool attacking)
    {
        if (attacking) { attackers.LoadArmy(a); }
        else { defenders.LoadArmy(a); }
    }
    void Refresh()
    {
        LoadArmy(attacker, true);
        LoadArmy(defender, false);
    }
    public void HandleResolve()
    {
    }
    public void HandleSkip()
    {
        attacker.BattleAnother(defender, false);
        Refresh();
    }
    public void HandleFlee()
    {
        Close();
    }
}
