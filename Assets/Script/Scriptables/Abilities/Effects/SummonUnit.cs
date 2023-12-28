using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon Ally", menuName = "Abilities/Effects/Summon Ally")]
public class SummonUnit : AbilityEffect
{
    public enum Alignment
    {
        player,
        enemy,
        ownerside,
        neutral,
    }
    public int monsterAmount = 1;
    public Alignment monsterAlignment = Alignment.ownerside;
    public GameObject summonPrefab;
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        var room = table.caster.transition.CurrentRoom;
        if (room != null && summonPrefab != null)
        {
            Vector2 summonPoint = table.point;
            if (table.ability.AbilityBehavior == AbilityDefines.Behavior.self)
                summonPoint = table.origin;

            bool Allied = table.caster.IsPlayerControlled();


            switch (monsterAlignment)
            {
                case Alignment.player:
                    Allied = true;
                    break;
                case Alignment.enemy:
                    Allied = true;
                    break;
            }
            for (int i = 0; i < monsterAmount; i++) {
                Mob mob = room.spawner.SpawnEnemyFromPrefab(summonPrefab, room.grid.RandomNodeInCircle(summonPoint, table.ability.GetAreaRange(false)), Allied);

                if (Allied)
                {
                    PlayerController.main.heroMan.SpawnHero(mob);
                }
            }
        }
    }
}
