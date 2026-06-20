using UnityEngine;

public class ArmyStatusComponent : ArmyComponent
{
    public int shieldedTurns=0;
    public int cloakedTurns =0;

    public ArmyStatusComponent(DataItemArmy parent) : base(parent)
    {
    }

    public void Shield(int turns)
    {
        shieldedTurns = GameManager.main.currentTurn + turns;
    }
    public bool IsShielded()
    {
        return shieldedTurns >= GameManager.main.currentTurn;
    }
    public void Cloak(int turns)
    {
        cloakedTurns = GameManager.main.currentTurn + turns;
    }
    public bool IsCloaked()
    { return cloakedTurns >= GameManager.main.currentTurn;
    }
}
