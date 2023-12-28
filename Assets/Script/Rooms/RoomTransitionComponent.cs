using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitionComponent : MobComponent
{

    public float ScreenTransitionTime = 1f; 

    public EnvironmentController CurrentRoom;
    public void TeleportToPoint(string pointName, bool skipCoroutine = false)
    {
        if (LevelController.main != null && LevelController.main.GetPointByName(pointName) != null)
        {
            TeleportToPoint( LevelController.main.GetPointByName(pointName), skipCoroutine);
        }
    }
    public void TeleportToPoint(NavPoint roomPoint, bool skipCoroutine = false)
    {
        if (roomPoint!=null)
        {
            roomPoint.FindRoom();
        }
            if (roomPoint.room != null)
            {
                TransitionRoom(roomPoint.room, roomPoint.GetRelativePosition(), skipCoroutine);
            }
        
    }
    public void TransitionRoom(string newRoomName, Vector2 pointPosition, bool skipCoroutine = false)
    {
        if (LevelController.main != null && LevelController.main.GetRoomByName(newRoomName) != null)
        {
            TransitionRoom(LevelController.main.GetRoomByName(newRoomName), pointPosition, skipCoroutine);
        }
    }
    Coroutine TransitionCoroutine;
    public void TransitionRoom(EnvironmentController newRoom, Vector2 pointPosition, bool skipCoroutine = false)
    {
        if (newRoom == CurrentRoom)
            return;

        if (skipCoroutine)
        {
            MovePlayerToNewRoom(newRoom, pointPosition);
            return;
        }
        if (TransitionCoroutine!=null)
        {
            StopCoroutine(TransitionCoroutine);
        }
        TransitionCoroutine = StartCoroutine(FadeCoroutine(newRoom, pointPosition));
    }
    public IEnumerator FadeCoroutine(EnvironmentController newRoom, Vector2 pointPosition)
    {
        yield return new WaitForEndOfFrame();
        /*if (UIController.main.TransitionScreen!=null)
        {
            UIController.main.TransitionScreen.StopAllCoroutines();
            yield return UIController.main.TransitionScreen.AwaitTransitionIn(ScreenTransitionTime);
        }*/
        MovePlayerToNewRoom(newRoom, pointPosition);
        /*if (UIController.main.TransitionScreen != null)
        {
            yield return UIController.main.TransitionScreen.AwaitTransitionOut(ScreenTransitionTime);
        }*/
    }
    void MovePlayerToNewRoom(EnvironmentController newRoom, Vector2 pointPosition)
    {
        Vector3 teleportPosition = newRoom.transform.position + (Vector3)pointPosition;
        MovePlayerToNewRoom(newRoom, newRoom.grid.GetClosestToPoint(newRoom.grid.TranslateCoordinate(teleportPosition)));
    }
    public void ExitCurrentRoom()
    {
        if (CurrentRoom != null)
        {
            CurrentRoom.OnMobExitRoom(parent);
        }
    }
        public void MovePlayerToNewRoom(EnvironmentController newRoom, GridNav.Node point)
    {
        ExitCurrentRoom();
        CurrentRoom = newRoom;
        CurrentRoom.OnMobEnterRoom(parent);

        if (parent.movement != null)
        {
            parent.movement.Stop();
        }
        if (parent.pathfinder != null)
        {
            parent.pathfinder.ChangeGrid(newRoom.grid, point);
            parent.pathfinder.MoveToNode(point);
        }
        parent.OnRoomChange();
    }
}
