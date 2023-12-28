using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffectController : MonoBehaviour
{
    #region Rotation
    public enum RotationBehavior
    {
        nothing,
        arrow,
        lobbed
    }
    public float lingerDuration = 0;
    public RotationBehavior rotationBehavior;
    public GameObject explodeEffect;
    void AdjustRotation()
    {
        switch (rotationBehavior)
        {
            case RotationBehavior.arrow:
                transform.right = (Vector2)velocity;
                break;
            case RotationBehavior.lobbed:
                float fwdAngle = Mathf.Atan2(velocity.y, (velocity.x > 0 ? -1 : 1) * velocity.x) * Mathf.Rad2Deg;
                transform.eulerAngles = Vector3.forward * fwdAngle;
                break;
        }
    }
    #endregion
    ParticleSystem[] particles;

    protected virtual void Awake()
    {
        List<ParticleSystem> pSys = new List<ParticleSystem>();
        if (TryGetComponent<ParticleSystem>(out ParticleSystem mine))
        pSys.Add(mine);
        pSys.AddRange(GetComponentsInChildren<ParticleSystem>());
        particles = pSys.ToArray();
    }
    void Recolor( Color color)
    {
        foreach (ParticleSystem p in particles)
        {
            ParticleSystem.MainModule pmain = p.main;
            pmain.startColor = color;
        }
    }
    #region Fire Point
    public virtual void LaunchTowardsPosition(Vector3 start, Vector3 destination, Color color, float delay = 0, float travelSpeed = 1, bool rotating = true)
    {
        gameObject.SetActive(true);
        Recolor(color);
        transform.position = start;
        StartCoroutine(FirePosition(delay, travelSpeed, destination));
    }
    Vector3 velocity;
    protected virtual IEnumerator FirePosition(float delay, float projectileSpeed, Vector3 destination)
    {
        Vector3 origin = transform.position;
        velocity = (destination - origin) ;

        ChangeVisibility(false);
        yield return new WaitForSeconds(delay);
        ChangeVisibility(true);
        for (float t = 0; t <= 1; t += Time.fixedDeltaTime / projectileSpeed)
        {
            transform.position = Vector3.Lerp(origin, destination, t);
            AdjustRotation();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(lingerDuration);
        Die();
    }
    #endregion
    #region Fire Target
    public virtual void LaunchAtTarget(Vector3 start, Transform target, Color color, float delay = 0, float travelSpeed = 1, bool rotating = true)
    {
        gameObject.SetActive(true);
        foreach (ParticleSystem p in particles)
        {
            ParticleSystem.MainModule pmain = p.main;
            pmain.startColor = color;
        }
        transform.position = start;
        StartCoroutine(FireTarget(delay, travelSpeed, target));
    }
    protected virtual IEnumerator FireTarget(float delay, float projectileSpeed, Transform target)
    {
        Vector3 origin = transform.position;
        velocity = (target.transform.position - origin);

        ChangeVisibility(false);
        yield return new WaitForSeconds(delay);
        ChangeVisibility(true);

        for (float t = 0; t <= 1; t += Time.fixedDeltaTime / projectileSpeed)
        {
            transform.position = Vector3.Lerp(origin, target.position, t);
            AdjustRotation();
            yield return new WaitForEndOfFrame();
        }
        AttachToObject(target);
        yield return new WaitForSeconds(lingerDuration);
        Die();
    }
    #endregion
    void AttachToObject(Transform target)
    {
        transform.SetParent(target);
        transform.localScale = Vector3.one;
    }
    bool Visible = false;
    void ChangeVisibility(bool visible)
    {
        Visible = visible;

        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer mSprite))
            mSprite.enabled = visible;
        foreach (ParticleSystem p in particles)
        {
            if (!visible)
                p.Stop();
            else
                p.Play();
        }
    }
    public void Die()
    {
        ChangeVisibility(false);
        if (explodeEffect != null)
            SpecialEffectPool.main.EffectFromPrefab(explodeEffect, transform.position,0,1);
        SpecialEffectPool.main.DeactivateObject(gameObject);
    }

    /*Projectile

    public static GameObject Emit(Vector3 start, Vector3 end, float duration, float delay, Color color, string projprefab)
    {

        GameObject projectile = SpecialEffectPool.EffectFromPrefab(projprefab, start, delay, color);
        if (projectile == null)
        {
            UnityEngine.Debug.LogWarning("Prefab " + projprefab + " does not exist!");
            return null;
        }

        ProjectileEffectController projcont = projectile.GetComponent<ProjectileEffectController>();
        projcont.duration = duration;
        projcont.delay = delay;
        projcont.endPosition = end;

        return projectile;
    }

    public static SpecialEffectController[] ProjectileCircle(Vector3 start, float radius, int nProjectiles, float delay, float duration, Color color, string projtextname, string explosiontexname)
    {
        return ProjectileCircle(start, radius, nProjectiles, delay, 0, duration, color, projtextname, explosiontexname);
    }

    public static SpecialEffectController[] ProjectileCircle(Vector3 start, float radius, int nProjectiles, float delay, float delayrnd, float duration, Color color, string projtextname, string explosiontexname)
    {
        return ProjectileCone(start, 0, radius, nProjectiles, 360, delay, delayrnd, duration, color, projtextname, explosiontexname);
    }


    public static SpecialEffectController[] ProjectileCone(Vector3 start, Vector3 end, int nProjectiles, float coneangle, float delay, float duration, Color color, string projtextname, string explosiontexname)
    {
        return ProjectileCone(start, end, nProjectiles, coneangle, delay, 0, duration, color, projtextname, explosiontexname);
    }

    public static SpecialEffectController[] ProjectileCone(Vector3 start, Vector3 end, int nProjectiles, float coneangle, float delay, float randomdelay, float duration, Color color, string projtextname, string explosiontexname)
    {
        Vector3 delta = end - start;
        return ProjectileCone(start, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg, delta.magnitude, nProjectiles, coneangle, delay, randomdelay, duration, color, projtextname, explosiontexname);
    }

    public static SpecialEffectController[] ProjectileCone(Vector3 start, float startang, float conerange, int nProjectiles, float coneangle, float delay, float duration, Color color, string projtextname, string explosiontexname)
    {
        return ProjectileCone(start, startang, conerange, nProjectiles, coneangle, delay, 0, duration, color, projtextname, explosiontexname);
    }

    public static SpecialEffectController[] ProjectileCone(Vector3 start, float startang, float conerange, int nProjectiles, float coneangle, float delay, float randomdelay, float duration, Color color, string projtextname, string explosiontexname)
    {
        List<SpecialEffectController> total = new List<SpecialEffectController>();
        for (int Zim = 0; Zim < nProjectiles; Zim++)
        {
            float angle = startang - coneangle / 2 + Zim * (coneangle / nProjectiles);
            Emit(start, new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * conerange, duration, Random.value * randomdelay + delay, color, projtextname);
        }
        return total.ToArray();
    }*/
}
