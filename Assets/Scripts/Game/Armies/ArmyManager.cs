using System.Collections.Generic;

public class ArmyManager : EntityManager
{

    public List<DataItemArmy> armies = new();
    public HashSet<DataItemArmy> movingArmies = new();
}

public class EntityManager : GameComponent
{
    public virtual void HandleEndOfTurn()
    {

    }
}