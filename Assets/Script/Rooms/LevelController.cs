using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController main;
    public DungeonComponent StartingRoom;
    EnvironmentController[] rooms;   
    NavPoint    [] navPoints;
    public ObjectPool monsterPool;
    void Awake()
    {
        main = this;
        navPoints = GetComponentsInChildren<NavPoint>();
        rooms = GetComponentsInChildren<EnvironmentController>();
        InitVariableReactors();
    }
    private void Start()
    {
        BeginGame();
    }
    public void ResetRooms()
    {
        foreach (EnvironmentController room in rooms)
        {
            room.gameObject.SetActive(false);
        }
    }

    public NavPoint GetPointByName(string rName)
    {
        foreach (NavPoint p in navPoints)
        {
            if (p.PointID == rName)
                return p;
        }
        return null;
    }
    public EnvironmentController GetRoomByName(string rName)
    {
        foreach (EnvironmentController r in rooms)
        {
            if (r.name == rName)
                return r;
        }
        return null;
    }
    public EnvironmentController GetRoomByPoint(Vector2 point)
    {
        foreach (EnvironmentController r in rooms)
        {
            if (r.grid.TryGetWorldNodeAt(point, out GridNav.Node n))
                return r;
        }
        return null;
    }
    public UnityEvent OnGameStarted;
    public void BeginGame()
    {
        OnGameStarted.Invoke();
    }
    #region Variable Reactors

    public List<ConditionalActionTrigger > Reactors = new List<ConditionalActionTrigger >();

    public void RegisterReactor(ConditionalActionTrigger  reactor)
    {
        if (!Reactors.Contains(reactor))
            Reactors.Add(reactor);
    }

    void InitVariableReactors()
    {

        Reactors = new List<ConditionalActionTrigger >();

        foreach (ConditionalActionTrigger  posReactor in Resources.FindObjectsOfTypeAll(typeof(ConditionalActionTrigger )) as ConditionalActionTrigger [])
            if (posReactor.gameObject.scene.IsValid())
                RegisterReactor((ConditionalActionTrigger )posReactor);
    }

    #endregion
}
