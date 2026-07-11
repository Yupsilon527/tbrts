using UnityEngine;

public class CombatWindow : Window
{
    DataItemBanner attacker, defender;
    public CombatSide attackers, defenders;
    public GameObject buttonsParent;
    protected override void OnClosed()
    {
        attackers.Clear(); defenders.Clear();
        base.OnClosed();
    }
    public void PresentSides(DataItemBanner a, DataItemBanner d)
    {
        attacker = a;
        defender = d;
        LoadArmy(a, true);
        LoadArmy(d, false);
    }
     void LoadArmy(DataItemBanner a, bool attacking)
    {
        if (attacking) { attackers.LoadArmy(a); }
        else { defenders.LoadArmy(a); }
    }
    public void HandleResolve()
    {
        attacker.BattleAnother(defender, false);
        Close();
    }
    public void HandleFlee()
    {

    }
}
