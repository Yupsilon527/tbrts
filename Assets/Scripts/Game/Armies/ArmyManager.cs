using System.Collections.Generic;

public class ArmyManager : EntityManager
{
    public DataItemArmy mainSelectedArmy = null;
    public List<DataItemArmy> armies = new();
    public HashSet<DataItemArmy> movingArmies = new();
}

public class EntityManager : GameComponent
{
    public virtual void HandleEndOfTurn()
    {

    }
}