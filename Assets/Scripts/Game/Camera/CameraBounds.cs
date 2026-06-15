using UnityEngine;

public class CameraBounds : Initializable
{
    public Rect rBounds = new Rect (0,0,0,0);
    public float maxScale = 8;
    private void OnValidate()
    {
        SetRect( new Rect(transform.position.x - transform.localScale.x * .5f, transform.position.y - transform.localScale.y * .5f, transform.localScale.x, transform.localScale.y));
    }

    public void SetRect(Rect rbounds)
    {
        rBounds = rbounds;
        maxScale = Mathf.Max(rBounds.width, rBounds.height) / 2;
    }
    private void OnDrawGizmosSelected()
    {
        if (!inspect) return;
        Gizmos.color = Color.white;
        Gizmos.DrawCube(new Vector3(rBounds.center.x,rBounds.center.y,0),new Vector3(rBounds.width,rBounds.height,1));
    }
}
