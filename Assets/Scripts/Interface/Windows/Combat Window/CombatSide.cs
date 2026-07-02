using UnityEngine;

public class CombatSide : MonoBehaviour
{
    public CombatantContainer transporter;
    public CombatantContainer[] formation;
    public GameObject supportParent;
    public CombatantContainer[] support;

    public void LoadArmy(DataItemArmy army)
    {
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
    public void Clear()
    {
        transporter.gameObject.SetActive(false);
        transporter.Clear();
        foreach (var u in formation)
        {
            u.Clear();   u.gameObject.SetActive(false);
        }
        supportParent.SetActive(false);
        foreach (var ob in support)
        {
            ob.Clear(); ob.gameObject.SetActive(false);
        }
    }
}
