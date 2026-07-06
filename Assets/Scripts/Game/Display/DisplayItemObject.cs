using System;
using System.Collections.Generic;
using UnityEngine;
public enum DisplayPositionChange
{
    instant,
    teleport,
    move
}

public class DisplayItemObject<tDataItem> : Initializable ,  IDisplayItemObject<tDataItem>  where tDataItem : DataItemObject
{
    public tDataItem assignedObject;
    public tDataItem AssignedObject => assignedObject;

    public GameObject SpritePrefab;
    public List<SpriteRenderer> objectSprites, bannerSprites, selectionCircles;
    public DisplayBanner banner;
    public float movementSpeed = 1;
    Vector3 direction;
    float realSpeed, approachDist;
    List<Vector3> destinations = new();
    public virtual void AssignObject(tDataItem ob)
    {
        assignedObject = ob;
        ob.display = this;
    }
    public virtual void DrawFresh()
    {
        OnGraphicsChange();
    }
    public virtual void DrawAgain()
    {
    }
    public virtual void OnPathChange()
    {
    }
    public virtual void OnGraphicsChange()
    {
        OnVisibilityChange();
        DrawAgain();
    }
    public virtual void OnVisibilityChange()
    {
        gameObject.SetActive(IsVisible());
    }
    public virtual bool IsVisible()
    {
        return assignedObject.IsVisibleToPlayer(GameManager.main.playerManager.currentPlayer);
    }
    public virtual void OnPlayerOwnerChange()
    {
        banner?.ChangePlayer(assignedObject.GetPlayerOwner());
    }
    public virtual void OnSelectionChange()
    {
        bool selected = assignedObject.IsSelected();
        foreach (var sprite in selectionCircles)
        {
            sprite.enabled = selected;
        }
    }
    public  void OnPositionChange(Vector2Int gridPos, DisplayPositionChange change)
    {
        OnPositionChange(SidewaysMap.main.TranslateEntityPosition(gridPos),change);
    }
    public virtual void OnPositionChange(Vector3 pos, DisplayPositionChange change)
    {
        switch (change)
        {
            case DisplayPositionChange.instant:
                transform.position = pos;
                break;
            case DisplayPositionChange.move:
                QueuePoint(pos);
                break;
        }
    }

    private void Update()
    {
        if (destinations.Count == 0) {  return; }

        var destination = destinations[0];
        realSpeed = movementSpeed * Time.deltaTime;
        approachDist = (transform.position - destination).sqrMagnitude;
        if (approachDist < realSpeed * realSpeed)
        {
            OnPositionChange( destination,DisplayPositionChange.instant);
            destinations.RemoveAt(0);
            if (destinations.Count == 0)
            {
                Stop();
            }
            else
            {
                MoveToNextPoint(destinations[0]);
            }
            return;
        }
        transform.position += direction * realSpeed;
    }
    public void MoveToPoint(Vector3 point, bool clean = false)
    {
        if (clean)
            destinations.Clear();
        QueuePoint(point);
    }
    public void QueuePoint(Vector3 point)
    {
        if (destinations.Count == 0)
        {
            MoveToNextPoint(point);
        }
        destinations.Add(point);
    }
    public void MoveToNextPoint(Vector3 point)
    {
        direction = (point - transform.position).normalized;

       /* if (parent.animator.GetBool("dead"))
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (facesLeft ? 1 : -1), -Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }
        else if (Mathf.Abs(point.x - transform.position.x) > .05f)
        {
            bool oldDir = facesLeft;
            SetFacing(direction.x < 0);
            if (facesLeft != oldDir)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (facesLeft ? 1 : -1), Mathf.Abs(transform.localScale.y), transform.localScale.z);
                parent.animator.SetTrigger("turn");//TODO define
            }
        }
        Vector2 faceDirection = new Vector2(direction.x, direction.y * (facesLeft ? -1 : 1));
        desiredRot = Mathf.Atan2(faceDirection.y, faceDirection.x) * Mathf.Rad2Deg * .5f;
        desiredRot = Mathf.Clamp(desiredRot, -rotationMaximum, rotationMaximum);

        if (parent.animator.GetBool("dead"))
            parent.animator.SetFloat("swimmingSpeed", direction.y);
        else
            parent.animator.SetFloat("swimmingSpeed", (speedCoefficient + startingSpeed) / speedCoefficient);*/

        Inspect("Moves to destination " + point);
    }
    public void Stop()
    {
        Inspect("Stop Moving");
        realSpeed = 0;
    }
    public  GameObject GetParentObject()
    {
        return gameObject;
    }
}
public interface IDisplayItemObject<out T> where T : DataItem
{
    T AssignedObject { get; }
    public abstract GameObject GetParentObject();
    public abstract void DrawFresh();
    public abstract void DrawAgain();
    public abstract void OnPlayerOwnerChange();
    public abstract void OnSelectionChange();
    public abstract void OnPositionChange(Vector2Int gridPos, DisplayPositionChange change);
    public abstract void OnGraphicsChange();
    public abstract void OnVisibilityChange();
    public abstract void OnPathChange();
}