using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectPool : ObjectPool
{
    public GameObject textEffectPrefab;
    public static SpecialEffectPool main;
    private void Awake()
    {
        main = this;
    }

    GameObject PoolEffect(GameObject prefab,  float delay = 0)
    {
        GameObject effect = PoolItem(prefab);
        if (effect == null)
            return null;

        ActivateObject(effect);
        if (effect.TryGetComponent(out SpecialEffectController sec)
            )
        {
            sec.Emit(  delay);
        }

        return effect;
    }

    public GameObject EffectFromPrefab(GameObject prefab,  Vector3 pos, float delay = 0, float scale = 1)
    {
        GameObject effect = PoolEffect(prefab,  delay);
        if (effect == null)
            return null;

        effect.transform.position = pos;
        effect.transform.localScale = Vector3.one * scale;
        return effect;
    }

    public GameObject AttachEffectFromPrefab(GameObject parent, GameObject prefab, float delay = 0, float scale = 1)
    {
        GameObject effect = PoolEffect(prefab, delay);
        if (effect == null)
            return null;

        effect.transform.parent = parent.transform;

        effect.transform.localPosition = Vector3.one;
        effect.transform.localScale = Vector3.one * scale;
        effect.transform.localRotation = Quaternion.identity;

        return effect;
    }
    public GameObject EffectFromPrefabOnEntity(Mob target, GameObject prefab, string attachPoint = "origin", float delay = 0, float scale = 1)
    {
        GameObject effect = PoolEffect(prefab,  delay);
        if (effect == null)
            return null;
        HeroAttachPoint atp = target.GetComponent<AnimationComponent>().FindAttachPoint(attachPoint);
        if (atp != null)
        {
            effect.transform.position = atp.transform.position;
            effect.transform.forward = atp.transform.forward;
        }
        effect.transform.localScale = Vector3.one * scale;
        return effect;
    }
    public GameObject AttachEffectFromPrefabOnEntity(Mob target, GameObject prefab, string attachPoint = "origin", float delay = 0, float scale = 1)
    {
        GameObject effect = PoolEffect(prefab, delay);
        if (effect == null)
            return null;
        HeroAttachPoint atp = target.GetComponent<AnimationComponent>().FindAttachPoint(attachPoint);
        if (atp != null)
        {
            effect.transform.parent = atp.transform;

            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
        }
        effect.transform.localScale = Vector3.one * scale;
        return effect;
    }

    public void TextEffect(string text, Vector3 position, float delay = 0)
    {
        GameObject effect = PoolEffect(textEffectPrefab, delay);
        if (effect == null)
            return;
        effect.transform.localPosition = position + Vector3.left * 2;
        if (effect.TryGetComponent(out TextEffectController tefX))
        {
            tefX.ChangeTextValue(text);
        }
    }
}
