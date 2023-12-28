using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectController : MonoBehaviour
{
    protected ParticleSystem[] particles;
    protected Animator animator;
    protected virtual void Awake()
    {
        List<ParticleSystem> pSys = new List<ParticleSystem>();
        if (TryGetComponent(out ParticleSystem particle))
        pSys.Add(particle);
        pSys.AddRange(GetComponentsInChildren<ParticleSystem>());
        particles = pSys.ToArray();
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    public virtual void Emit( float delay)
    {
         StartCoroutine(EmitOnce(delay));
    }
    protected virtual IEnumerator EmitOnce(float delay)
    {
        reps = 0;
        if (delay>0)
         yield return new WaitForSeconds(delay);
        Play();
    }

    public virtual void Emit( float delay, int repeats)
    {
        float duration = 0;
        foreach (ParticleSystem p in particles)
        {
            ParticleSystem.MainModule pmain = p.main;
            //pmain.startColor = color;
            //pmain.startSpeedMultiplier = scale;
            duration = Mathf.Max(pmain.duration + pmain.startLifetime.constantMax);
        }
        StartCoroutine(EmitMultiple(delay, duration, repeats));
    }
    int reps = 0;
    protected virtual IEnumerator EmitMultiple(float delay, float duration, int repeats)
    {
        reps = repeats;
        yield return new WaitForSeconds(delay);
        while (reps > 0)
        {
            Play();
            reps--;
                yield return new WaitForSeconds(duration);
        }
    }
    protected void Play()
    {
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        if (animator != null)
            animator.SetTrigger("Play");
    }
    public void Stop()
    {
            foreach (ParticleSystem p in particles)
            {
                p.Stop();
            }
            SpecialEffectPool.main.DeactivateObject(gameObject);
        if (attachedModifier!=null)
        {
            attachedModifier.DetachParticle(this);
        }
        
    }
    private void OnParticleSystemStopped()
    {
        if (reps == 0)
        {
            Stop();
        }
    }
    PropertyModifier attachedModifier;
    public void AttachToModifier(PropertyModifier mod)
    {
        if (attachedModifier == null)
            attachedModifier = mod;
    }
}

