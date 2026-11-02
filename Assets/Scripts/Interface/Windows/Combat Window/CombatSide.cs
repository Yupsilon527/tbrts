using System.Linq;
using UnityEngine;

public class CombatSide : MonoBehaviour
{
    DataItemBanner assigned;
    public CombatantContainer transporter;
    public CombatantContainer boss;
    public CombatantContainer[] formation;
    public GameObject supportParent;
    public CombatantContainer[] support;
    public HealthBar healthBar;

    public void LoadArmy(DataItemBanner army)
    {
        Clear();
        if (army == null)
            return;
        assigned = army;
        var units = army.formation.Formation.Where(u => u != null).ToList();

        if (units.Count == 1 && units[0].IsSuperUnit())
        {
            boss.gameObject.SetActive(true);
            boss.ForUnit(units[0]);
        }
        else
        {
            boss.gameObject.SetActive(false);
            if (army.formation.transport != null)
            {
                transporter.gameObject.SetActive(true);
                transporter.ForUnit(army.formation.transport);
            }
            else { transporter.gameObject.SetActive(false); }

            for (int i = 0; i < army.formation.Formation.Length; i++)
            {
                var unit = army.formation.Formation[i];
                if (unit != null)
                {
                    formation[i].gameObject.SetActive(true);
                    formation[i].ForUnit(unit);
                }
                else { formation[i].gameObject.SetActive(false); }
            }
        }

        var allies = army.GetSupportingUnits();
        supportParent.SetActive(allies.Length > 0);
        for (int i = 0; i < support.Length; i++)
        {
            if (i < allies.Length)
            {
                support[i].gameObject.SetActive(true);
                support[i].ForUnit(allies[i]);
            }
            else { support[i].gameObject.SetActive(false); }
        }
        Refresh();
    }
    public void Refresh()
    {
        if (healthBar != null)
        {
            float health = assigned.formation.GetHealthCurrent();
            float limit = assigned.formation.GetHealthTotal();
            float percent = health / limit;

            healthBar.ShowValue((percent * 100).ToString("F0"), percent);
        }
    }
    public void Clear()
    {
        transporter.gameObject.SetActive(false);
        transporter.Clear();
        foreach (var u in formation)
        {
            u.Clear(); u.gameObject.SetActive(false);
        }
        supportParent.SetActive(false);
        foreach (var ob in support)
        {
            ob.Clear(); ob.gameObject.SetActive(false);
        }
    }
}
