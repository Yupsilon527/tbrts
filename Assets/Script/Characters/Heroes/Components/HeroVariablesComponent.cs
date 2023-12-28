using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroVariablesComponent : HeroComponent, IEntityEvent
{
    #region Event Reactions

    public void EventReaction(AbilityDefines.Event evt, Mob[] affectedCritters)
    {
        switch (evt)
        {
            case AbilityDefines.Event.OnScoreKill:
                foreach (Mob victim in affectedCritters)
                {
                    if (victim is Monster monster)
                    {
                        //local scope
                        PlayerController.main.party.scope.ChangeVariable("monster_kills_total", Variables.Change.Case.add, 1);
                        PlayerController.main.party.scope.ChangeVariable($"monster_{monster.monsterData.InternalName}_killed", Variables.Change.Case.add, 1);


                        if (monster.monsterLoot!=null)
                        {
                            monster.monsterLoot.GrantRewards();
                        }
                    }
                }
                break;
            case AbilityDefines.Event.OnDeath:
                PlayerController.main.party.scope.ChangeVariable("deaths", Variables.Change.Case.add, 1);
                break;
                //TODO afktime
        }
    }
    #endregion
}
