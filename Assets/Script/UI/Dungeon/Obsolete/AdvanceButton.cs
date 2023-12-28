using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdvanceButton : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed;

    public void Update()
    {
        if (isPressed)
        {
            PlayerController.main.party.HandleAdvance(Time.fixedDeltaTime);
        }
    }
    public void OnPointerDown(PointerEventData data)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        isPressed = false;
    }
}
