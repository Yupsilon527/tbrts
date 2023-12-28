using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public bool OOBclose = true;
    public bool IsOpen()
    {
        return isActiveAndEnabled;
    }
    public virtual void Open()
    {
        if (!isActiveAndEnabled)
        {
            gameObject.SetActive(true);
        }
    }
    public virtual void Close()
    {
        if (isActiveAndEnabled)
        {
            gameObject.SetActive(false);
        }
    }
    public virtual void OnDisable()
    {

    }
}
