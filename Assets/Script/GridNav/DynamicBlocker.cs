using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBlocker : MonoBehaviour
{
    public GridNav grid;
    public Collider2D collider;
    GridNav.Node[] nodes;

    private void OnEnable()
    {
        if (grid == null) return;

        if (nodes == null || nodes.Length == 0)
        {
            

            nodes = grid.GetNodesInBox(transform.position - .5f * transform.localScale, transform.position + .5f * transform.localScale);
        }
        collider.enabled = true;
        ReviseCollision();
    }
    private void OnDisable()
    {
        collider.enabled = false;
        ReviseCollision();
    }
    void ReviseCollision()
    {
        foreach (GridNav.Node node in nodes)
        {
            node?.UpdateCollision(grid);
        }
    }
}
