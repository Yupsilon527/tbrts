using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoomController : MonoBehaviour
{
    public EnvironmentController ActiveRoom;
    public void ChangeRoom(EnvironmentController nRoom)
    {
        DisableCurrentRoom();
        ActiveRoom = nRoom;
        EnableCurrentRoom();
    }
    public void EnableCurrentRoom()
    {
        if (ActiveRoom != null )
        {
            if (!ActiveRoom.gameObject.activeSelf)
            ActiveRoom.gameObject.SetActive(true);
            //TODO ActiveRoom.spawner.OnGameResume();
            if (ActiveRoom.bounds != null)
                CameraController.main.SetBounds(ActiveRoom.bounds);
        }
    }
    public void DisableCurrentRoom()
    {
        if (ActiveRoom != null)
        {
            ActiveRoom.gameObject.SetActive(false);
            //TODO ActiveRoom.spawner.OnGamePause(Time.realtimeSinceStartup);
        }
        else
        {
            LevelController.main.ResetRooms();
        }
    }
}
