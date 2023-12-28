using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController main;

    [Header("Components")]
    public PlayerResourceManager resources;
    public DungeonCharacterManager party;
    public PlayerCardsController cards;
    public PlayerHeroController heroMan;
    public InventoryComponent sharedInventory;
    public PlayerVariableManager varMan;

    private void Awake()
    {
        main = this;

        if (party == null)
            party = GetComponent<DungeonCharacterManager>();

        if (resources == null)
            resources = GetComponent<PlayerResourceManager>();
        if (cards == null)
            cards = GetComponent<PlayerCardsController>();
        if (heroMan == null)
            heroMan = GetComponent<PlayerHeroController>();
        if (sharedInventory == null)
            sharedInventory = GetComponent<InventoryComponent>();
        if (varMan == null)
            varMan = GetComponent<PlayerVariableManager>();
    }
    public PlayerScope GetGlobalScope()
    {
        return varMan.variables            ;
    }
    public Variables.VariableScope GetDungeonScope()
    {
       return  party.scope;
    }
    #region Hubword/Dungeon
    public void TransitionToDungeon(Mob[] team, DungeonComponent dungeon)
    {
        party.PrepareTeamForDungeon(dungeon, team);
        dungeon.BeginDungeon();
    }
    
    #endregion
}
