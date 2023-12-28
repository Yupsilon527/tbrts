using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    CameraBounds currentBounds;
    public static CameraController main;

    private void Awake()
    {
        main = this;
        if (camera == null)
            camera = GetComponent<Camera>();
        if (room == null)
            room = GetComponent<CurrentRoomController>();
        Readjust();
    }
    public void SetBounds(CameraBounds newbounds)
    {
        currentBounds = newbounds;
    }
    public void DungeonCameraBehavior()
    {
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, .1f, Vector2.zero))
        {
            if (hit.transform.TryGetComponent(out CameraEventTrigger target))
            {
                target.CameraTouched();
            }
        }
        /*   Vector2 center = Vector2.zero;
           foreach (Hero hero in PlayerController.main.party.myCharacters)
           {
               center.x = Mathf.Max(hero.transform.position.x, center.x);
           }

           MovePosition(center);*/
    }
    public void JumptoMob(Mob followChar)
    {
        if (followChar == null) return;

        if (followChar.transition.CurrentRoom != room.ActiveRoom)
        {
            room.ChangeRoom(followChar.transition.CurrentRoom);
        }

        MovePosition(followChar.transform.position);
    }
    public void MoveDirection(Vector2 direction)
    {
        MovePosition((Vector2)transform.position + direction);
    }
    public void MovePosition(Vector2 center)
    {
        if (currentBounds == null || camera == null) return;
        if (width * 2 > currentBounds.rBounds.width)
        {
            center.x = currentBounds.rBounds.center.x;
        }
        else
        {
            center.x = Mathf.Clamp(center.x, currentBounds.rBounds.xMin + width, currentBounds.rBounds.xMax - width);
        }
        if (height * 2 > currentBounds.rBounds.height)
        {
            center.y = currentBounds.rBounds.center.y;
        }
        else
        {
            center.y = Mathf.Clamp(center.y, currentBounds.rBounds.yMin + height, currentBounds.rBounds.yMax - height);
        }
        transform.position = center;
    }
    float width = 0;
    float height = 0;
    void Readjust()
    {
        if (camera == null) return;
        height = camera.orthographicSize;
        width = height * camera.aspect;
    }

    public CurrentRoomController room;
    public Vector2 ScreenToWorldPoint(Vector2 screenPoint)
    {
        return camera.ScreenToWorldPoint(screenPoint);
    }
    public Mob MobFromWorldPoint(Vector2 worldPoint, bool playerOwned = false)
    {
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(worldPoint, .1f, Vector2.zero))
        {
            if (hit.transform.TryGetComponent(out Mob target))
            {
                if (!playerOwned || target.IsPlayerControlled())
                return target;
            }
        }
        return null;
    }
    public Mob MobFromScreenPoint(Vector2 screenPoint)
    {
        return MobFromWorldPoint(ScreenToWorldPoint(screenPoint));
    }
    public Mob MobFromWorldPointForAbility(Vector2 worldPoint, PropertyAbility ability)
    {
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(worldPoint, .1f, Vector2.zero))
        {
            if (hit.transform.TryGetComponent(out Mob target))
            {
                if (ability.CanCastOnTarget(target))
                    return target;
            }
        }
        return null;
    }
    public Mob MobFromScreenPointForAbility(Vector2 screenPoint,PropertyAbility ability)
    {
        return MobFromWorldPointForAbility(ScreenToWorldPoint(screenPoint), ability);
    }
    public void MoveCameraWithBorders(Vector2 screenPoint,float moveSpeed)
    {
        float screenBorders = Mathf.Min(Screen.width * .2f, Screen.height * .2f) ;
       Vector2 speed = Vector2.zero;
        if (screenPoint.x<screenBorders)
        {
            speed.x = -1;
        }
        else if (screenPoint.x > Screen.width - screenBorders)
        {
            speed.x = 1;
        }
        if (screenPoint.y < screenBorders)
        {
            speed.y = -1;
        }
        else if (screenPoint.y >  Screen.height - screenBorders)
        {
            speed.y = 1;
        }
        MoveDirection(speed * moveSpeed);
    }
}
