using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementIndicator : MonoBehaviour
{

    public SpriteRenderer originIndicator;
    public SpriteRenderer destinationIndicator;
    public LineRenderer lineRenderer;

    #region Origin and Destination
    public Vector2 origin;
    public Vector2 destination;

    Mob ownerMob;
    public void SetOwner(Mob mob)
    {
        ownerMob = mob;
    }
    void UpdateOrigin()
    {
        if (ownerMob != null)
        {
            origin = ownerMob.transform.position;
        }
    }

    protected virtual void Update()
    {
        UpdateOrigin();
        DrawLine();
    }
    protected void DrawLine()
    {
        if (originIndicator!=null)
        originIndicator.transform.position = origin;
        if (destinationIndicator != null)
            destinationIndicator.transform.position = destination;

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, destination);
        }
    }
    #endregion
    public virtual void ChangeColor(Color c)
    {
        originIndicator.color = c;
        destinationIndicator.color = c;

        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }
}

