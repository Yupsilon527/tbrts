using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializable : MonoBehaviour
{
    protected bool initialized = false;
    protected virtual bool Initialize()
    {
        if (initialized)
            return false;
        initialized = true;
        return true;
    }
    protected virtual void Awake()
    {
        Initialize();
    }
    protected virtual void OnEnable()
    {
        Initialize();
    }
}
