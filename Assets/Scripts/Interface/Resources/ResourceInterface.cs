
using UnityEngine;

public class ResourceInterface : MonoBehaviour
{
    public ResourceValueIndicator metalDoodle;
    public ResourceValueIndicator goldDoodle;
    public ResourceValueIndicator stoneDoodle;
    public ResourceValueIndicator techDoodle;
    public ResourceValueIndicator popDoodle;
   
    private void Start()
    {
     /*   var player = PlayerController.main.player;
        if (player != null)
        {
            var metal = player.GetResource(EconomyDefines.EconomyResource.Metal);
            metalDoodle.UpdateValue(EconomyDefines.EconomyResource.Metal, metal.GetValue());
            metal.OnValueChanged.AddListener(() =>
            {
                metalDoodle.UpdateValue(EconomyDefines.EconomyResource.Metal, metal.GetValue());
                DungeonInterfaceController.main.OnStuffChanged();
            });

            var gold = player.GetResource(EconomyDefines.EconomyResource.Gold);
            goldDoodle.UpdateValue(EconomyDefines.EconomyResource.Gold, gold.GetValue());
            gold.OnValueChanged.AddListener(() =>
            {
                goldDoodle.UpdateValue(EconomyDefines.EconomyResource.Gold, gold.GetValue());
                DungeonInterfaceController.main.OnStuffChanged();
            });

            var stone = player.GetResource(EconomyDefines.EconomyResource.Stone);
            stoneDoodle.UpdateValue(EconomyDefines.EconomyResource.Stone, stone.GetValue());
            stone.OnValueChanged.AddListener(() =>
            {
                stoneDoodle.UpdateValue(EconomyDefines.EconomyResource.Stone, stone.GetValue());
                DungeonInterfaceController.main.OnStuffChanged();
            });

            var tech = player.GetResource(EconomyDefines.EconomyResource.Knowledge);
            techDoodle.UpdateValue(EconomyDefines.EconomyResource.Knowledge, tech.GetValue());
            tech.OnValueChanged.AddListener(() =>
            {
                techDoodle.UpdateValue(EconomyDefines.EconomyResource.Knowledge, tech.GetValue());
                DungeonInterfaceController.main.OnStuffChanged();
            });

            var pop = player.GetResource(EconomyDefines.EconomyResource.Pop);
            popDoodle.UpdateValue(EconomyDefines.EconomyResource.Pop, player.GetPopulation()+"/"+player.GetPopulationLimit());
            pop.OnValueChanged.AddListener(() =>
            {
                popDoodle.UpdateValue(EconomyDefines.EconomyResource.Pop, player.GetPopulation() + "/" + player.GetPopulationLimit());
                DungeonInterfaceController.main.OnStuffChanged();
            });
        }*/
    }
}
