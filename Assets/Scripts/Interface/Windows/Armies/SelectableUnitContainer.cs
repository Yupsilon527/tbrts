using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectableUnitContainer : UnitPositionContainer
{
    public Image selectionCircle;
    public Action onClick;
    public virtual void SetSelected(bool  selected)
    {

    }
}
