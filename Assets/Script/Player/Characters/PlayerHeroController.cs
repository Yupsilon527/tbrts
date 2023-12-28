using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeroController : MonoBehaviour
{
    private void Start()
    { 
        SpawnHeroesFromLoadout();
        PlayerController. main.TransitionToDungeon(PlayerController.main.party.heroes.ToArray(), LevelController.main.StartingRoom);
        PlayerController.main.resources.InitStart();
    }
    #region Loadout
    public PlayerLoadout loadout;
    void SpawnHeroesFromLoadout()
    {
        DeckController.main.InitializeDeck();
        foreach (PlayerLoadout.HeroSlot heroSlot in loadout.heroSlots)
        {
            Hero newHero = SpawnHeroFromData(heroSlot.heroClass);
            foreach (AbilitySO card in heroSlot.cardLoadout)

            {
                DeckController
                    .main.AddCard(new PropertyCard(newHero, card));
            }
        }
        DungeonInterfaceController.main.portraits.InitalizeHeroes();

        DeckController.main.ShuffleDeck();
    }
    #endregion

    public Hero SpawnHeroFromData(HeroStatTableSO heroTable)
    {
        GameObject heroPrefab = Instantiate(heroTable.heroRig);
        if (heroPrefab.TryGetComponent(out Hero hero))
        {
            hero.classComponent.SetStats(heroTable);
            SpawnHero(hero);
            Debug.Log($"Hero from table {heroTable.name} has been created!");
            return hero;
        }
        return null;
    }
    public void SpawnHero(Mob hero)
    {
        PlayerController.main.party.heroes.Add(hero);
        hero.SetAlignment(true);
        hero.Spawn();
    }
    public void ForgetHero(Hero hero)
    {
        DungeonInterfaceController.main.portraits.UpdateButtonStates();
        PlayerHandController.main.DiscardCardsForHero(hero);
        DeckController.main.RemoveCardsByHero(hero);
    }
}
