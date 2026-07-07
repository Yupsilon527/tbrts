using TMPro;

public class UnitContainerDescriptipn : UnitContainer
{
    public TextMeshProUGUI unitStats;
    public TextMeshProUGUI unitAbilities;

    public override void ForUnit(DataItemUnit unit)
    {
        base.ForUnit(unit);
        if (unitStats != null)
        {
            unitStats.text = unit.OutputStatsTable();
        }
        if (unitStats != null)
        {
            unitStats.text = unit.OutputAbilityTable();
        }
    }
    public override void ForData(ProductionData data)
    {
        base.ForData(data);
        if (data is UnitData unit)
        {
            if (unitStats != null)
            {
                unitStats.text = unit.OutputStatsTable();
            }
            if (unitAbilities != null)
            {
                unitAbilities.text = unit.OutputAbilityTable();
            }
        }
    }
    public override void Clear()
    {
        base.Clear();
        if (unitStats != null)
            unitStats.text = "";
        if (unitAbilities != null)
            unitAbilities.text = "";
    }
}
