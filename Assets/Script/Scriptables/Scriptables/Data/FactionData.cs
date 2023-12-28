using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "Data/Faction")]
public class FactionData : ScriptableObject
{
    public ArmyData[] baseArmies;
    public BuildingData[] baseBuildings;
    public BaseData[] startingUnits;
}
