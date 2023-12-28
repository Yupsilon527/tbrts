using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar;

[RequireComponent(typeof(PathfinderComponent))]
public class MovementComponent : MobComponent, IOnIntrerupt
{
    public float MovementSpeed = 1f;

    public void Move(Vector2 destination)
    {
        parent.pathfinder.Resolve(destination, true);
        StartMovement();
    }
    public void Move(GridNav.Node destination)
    {
        parent.pathfinder.Resolve(destination);
        StartMovement();
    }
    public void Follow(GameObject target)
    {      
            parent.pathfinder.Follow(target);
            StartMovement();
        
    }
    void StartMovement()
    {
        Stop();
        if (SanityCheck())
            movementCoroutine = StartCoroutine(HandleMovement());
    }
    public void Stop(bool instant = true)
    {
        if (instant)
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            movementCoroutine = null;
        }
        else
        {
            walking = false;
        }
        AdjustZlevel();
    }
    Coroutine movementCoroutine;
    bool walking = true;
    IEnumerator HandleMovement()
    {
        walking = true;
        float wTime = 0;
    loopstart:
        if (SanityCheck() && walking && (!parent.combatant.enabled || !parent.combatant.IsInRange) && parent.pathfinder.Step() == PathfinderComponent.Failure.incomplete)
        {
                Vector3 start = transform.position;
                Vector3 end = (Vector3)parent.pathfinder.positionNode.node.worldPos ;
            end.z = end.y;
            parent.animations.SetAnimSpeed( MovementSpeed);

            FacesRight = start.x < end.x;

                while (wTime < 1)
                {
                    transform.position = Vector3.Lerp(start, end, wTime) ;
                    wTime += Time.deltaTime * MovementSpeed;
                    yield return new WaitForEndOfFrame();
                }
                wTime -= 1;
                goto loopstart;
            
        }

        Stop();
    }

    public bool IsWalking()
    {
        return movementCoroutine != null;
    }

    public void OnIntrerupt()
    {
        //Stop();
    }
    public GridNav.Node GetNode()
    {
        GridNav grid = parent.transition.CurrentRoom.grid;
        return grid.GetClosestToPoint(grid.TranslateCoordinate(transform.position));
    }
    public void AdjustZlevel()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
    public bool FacesRight = true;
}
