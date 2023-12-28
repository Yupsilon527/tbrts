using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemTile : MonoBehaviour
{
    public DataItemTile data;
    public DataItemWorld room;

    public GameObject highlight;
    public GameObject outline;

    public void InitData(DataItemTile data, Vector2Int worldPos)
    {
        //this.worldPos = new HexCoords(worldPos);
        data.coords.AxialCoords = worldPos;
        this.data = data;
        data.display = this;
        ReviseEntity();
    }
    void ReviseEntity()
    {
        /*if (data.LocatedEntity != null)
            data.LocatedEntity.movement.MoveToPosition(data.coords, 0, false);*/
    }
    public void SetHighlighted(bool value)
    {
        if (PlayerMouseInput.main != null)
        {
            if (value)
            {
                PlayerMouseInput.main.ChangeMouseTile( this);
            }
            else if (PlayerMouseInput.main.mouseOverTile == this)
            {
                PlayerMouseInput.main.ChangeMouseTile( null);
            }
        }
    }
    #region Visibility

    public bool IsVisible()
    {
        return false;//data.coords.DistanceFrom(EntityPlayer.main.movement.coords) <= WorldDefines.PlayerLos;

    }
    public void ChangeVisibility(bool value)
    {
        gameObject.SetActive(value);
    }

    #endregion

    #region Tile Colors
    Color TileColor;
     Color HighlightColor;
     Color OutlineColor;
    public enum tileState
    {
        clear = 0,
        transparent,
        highlight,
        walkpath,
        abilitycastable,
        abilitytarget,
        abilitytrajectory,
        enemyability,
        enemyattack,
        enemy,
    };

    public Color GetColor(tileState colorID)
    {
        switch (colorID)
        {
            case tileState.highlight:
                return new Color(1, 1, 1, .5f);
            case tileState.walkpath:
                return new Color(0, 0, 1, .33f);
            case tileState.enemyability:
                return new Color(1, 0, 0, .66f);
            case tileState.abilitycastable:
                return new Color(1, 0, 0, .33f);
            case tileState.enemyattack:
                return new Color(1, .5f, 0, .33f);
            case tileState.abilitytarget:
            case tileState.abilitytrajectory:
                return new Color(1, 0, 0, 1f);
            case tileState.enemy:
                return new Color(1, 0, 0, 1);
            case tileState.transparent:
                return new Color(1, 1, 1, .5f);
            default:
                return Color.clear;
        }
    }

    public void ChangeColor(tileState newState)
    {
        if (highlight != null)
        {
            TileColor = GetColor(newState);
            highlight.GetComponent<SpriteRenderer>().color = GetColor(newState);
        }
    }

    public void Highlight(tileState newState)
    {
        if (highlight != null)
        {
            HighlightColor = GetColor(newState);
            if (HighlightColor.a > 0)
            {
                highlight.GetComponent<SpriteRenderer>().color = HighlightColor;
            }
            else
            {
                highlight.GetComponent<SpriteRenderer>().color = TileColor;
            }
        }
    }
    public void ChangeOutline(tileState newState)
    {
        if (outline != null)
        {
            OutlineColor = GetColor(newState);
            
                outline.GetComponent<SpriteRenderer>().color = OutlineColor;
            
        }
    }
    #endregion
    
    private void OnEnable()
    {
        ChangeColor(tileState.clear);
        ChangeOutline(tileState.clear);
    }
    private void OnMouseEnter()
    {
        SetHighlighted(true);
    }
    private void OnMouseExit()
    {
        SetHighlighted(false);
    }
}
