using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceManager

    : WindowManager
{
    public static InterfaceManager main;
    //public UltimateBarElement ultimate;
    private void Awake()
    {
        main = this;
        /*
        if (ultimate == null)
            ultimate = GetComponentInChildren<UltimateBarElement>();*/
    }

    public bool IsMouseOverUI()
    {
        GraphicRaycaster gr = transform.GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        return results.Count > 0;
    }
}
