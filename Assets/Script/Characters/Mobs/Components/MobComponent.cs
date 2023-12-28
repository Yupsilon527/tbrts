using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobComponent : MonoBehaviour
{
    public Mob parent;
    protected virtual void Awake()
    {
        if (parent == null)
        {
            parent = GetComponent<Mob>();
        }
    }
    protected virtual bool SanityCheck()
    {
        return parent.gameObject.activeSelf && parent.damageable.isAlive();
    }
}
