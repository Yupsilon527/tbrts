using Unity.Burst.Intrinsics;
using UnityEngine;

public class GameManager : Initializable
{
    public static GameManager main;

    public DisplayPool displayPool;
    public ArmyManager armyManager;
    public CastleManager castleManager;
    public PlayerManager playerManager;

    public MapGen gen;
    public SidewaysMap map;

    public CameraBounds cb;
    public CameraController cc;

    public int currentTurn;
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
    }
    private void Start()
    {
        StartTheGame();
    }
    void StartTheGame()
    {
        playerManager.MakeThePlayersFromEditorData(gen.mapData.MapData);

        gen.GenerateMap();
        cc.SetBounds(cb);

        cb.SetRect(new Rect(TerrainDefines.UnitsPerTile * .5f, -TerrainDefines.UnitsPerTile * .5f, (map.width + 1) * TerrainDefines.UnitsPerTile, (map.height +1) * TerrainDefines.UnitsPerTile));
        castleManager.DrawTheCastlesFromEditorData(gen.mapData.MapData.castles);
        castleManager.redoCastleRegions();

        armyManager.GenerateTheArmiesFromEditorData(gen.mapData.MapData.armies);
        BeginNewTurn(true);
    }
    public void EndTurn()
    {
        playerManager.CycleActivePlayer();
    }
    public void BeginNewTurn(bool gamestart)
    {
        foreach (var castle in castleManager.buildings)
            castle.OnTurnBegin();
        foreach (var army in armyManager.armies)
            army.OnTurnBegin();
        if (gamestart) return;
        foreach (var player in playerManager.players)
            player.OnTurnBegin();
    }
    private void OnValidate()
    {
        FindComponent(ref cb);
        FindComponent(ref cc);
        FindComponent(ref displayPool);

        FindComponent(ref armyManager);
        FindComponent(ref castleManager);
        FindComponent(ref playerManager);
    }
}
