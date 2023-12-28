using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatAssistant : MonoBehaviour
{
    public static ThreatAssistant main;
    private void Awake()
    {
        main = this;
        enabled = false;
    }
    [Header("Threat Indicator")]
    public GameObject IndicatorPrefab;
    List<ThreatIndicator> indicatorList = new List<ThreatIndicator>();

    public ThreatIndicator PoolThreatIndicator(Vector2 point, float duration, float radius)
    {
        GameObject indicator = SpecialEffectPool.main.PoolItem(IndicatorPrefab);
        indicator.SetActive(true);
        if (indicator.TryGetComponent(out ThreatIndicator indi))
        {
            indi.transform.position = point;
            indi.SetDuration(duration);
            indi.SetRadius(radius);
            return indi;
        }
        return null;
    }

    public void OnDisable()
    {
        ClearIndicators();
    }
    void ClearIndicators()
    {
        foreach (ThreatIndicator indicator in indicatorList)
        {
            indicator.Destroy();
        }
        indicatorList.Clear();
    }
}
