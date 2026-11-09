using UnityEngine;

public class CombatWindow : Window
{
    public CombatantContainer[] containers;
    DataItemBanner attacker, defender;
    public CombatSide attackers, defenders;
    public AnimationController animations;
    public GameObject buttonsParent;
    private void Start()
    {
        containers = GetComponentsInChildren<CombatantContainer>(true);
    }
    protected override void OnClosed()
    {
        if (Combat.main.enabled)
        {
            Combat.main.FastForward();
        }
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
        attacker.BattleAnother(defender, false, false);
    }
    public void HandleSkip()
    {
        if (Combat.main.enabled)
        {
            Combat.main.FastForward();
        }
        else
        {
            attacker.BattleAnother(defender, false, true);
        }
        Refresh();
    }
    public void HandleFlee()
    {
        Close();
    }

    private void Update()
    {
        if (Combat.main.enabled && animations.animations.Count == 0)
        {
            Combat.main.OnGameTick();
        }
    }
}
