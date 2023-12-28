using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatIndicator : MonoBehaviour
{
    public Animator animator;
    public Transform innerCircle;
    public Transform outerCircle;

    public void SetRadius(float radius)
    {
        outerCircle.transform.localScale = Vector3.one * radius * 2;
    }
    public void SetDuration(float duration)
    {
        animator.SetFloat("duration", 1 / duration);
    }
    public void Destroy()
    {
        SpecialEffectPool.main.DeactivateObject(gameObject);
    }
}
