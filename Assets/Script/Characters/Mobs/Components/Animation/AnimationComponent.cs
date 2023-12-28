using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationComponent : MobComponent
{
    public Animator animator;

    protected override void Awake()
    {
        base.Awake();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        InitAttachPoints();
    }

    private void Update()
    {
        animator.SetBool("walking", parent.movement.IsWalking());
        animator.SetBool("faceright", parent.movement.FacesRight);
        animator.SetBool("dead", !parent.damageable.isAlive());
    }
    #region AttachPoints
    public Dictionary<string, HeroAttachPoint> Attachpoints;

    void InitAttachPoints()
    {
        Attachpoints = new Dictionary<string, HeroAttachPoint>();
        foreach (HeroAttachPoint atp in GetComponentsInChildren<HeroAttachPoint>())
        {
            if (!Attachpoints.ContainsKey(atp.AttachPointName))
            {
                Attachpoints.Add(atp.AttachPointName, atp);
            }
        }
    }
    public HeroAttachPoint FindAttachPoint(string searchName)
    {
        if (Attachpoints.ContainsKey(searchName))
        {
            return Attachpoints[searchName];
        }
        return null;
    }
    #endregion
    public void PlayAnimation(string triggerName, float aSpeed = 1)
    {
        animator.Play(triggerName) ;//TODO define
        SetAnimSpeed(1/aSpeed);
    }
    public void SetAnimSpeed(float speed)
    {
        animator.SetFloat("animSpeed", speed);
    }
}
