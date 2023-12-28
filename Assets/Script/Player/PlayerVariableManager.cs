using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVariableManager : MonoBehaviour
{
    public PlayerScope variables;
    private void Awake()
    {
        variables = new PlayerScope();
    }
}
