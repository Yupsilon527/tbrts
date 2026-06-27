using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarElement : MonoBehaviour
{
    public TextMeshProUGUI value;
    private void Awake()
    {
        UpdateProgress(0);
    }
    #region Progress Bar
    public Image Progressbar;
    public void UpdateProgress(float progress)
    {
        if (Progressbar!=null)
        Progressbar.fillAmount = Mathf.Clamp01(progress);
    }
    #endregion

    /*#region Boss Life
    [Header("Boss Healthbar")]
    public GameObject LifebarParent;
    public HealthBarController BossLifebar;

    public void EnableBossLifebar(bool value)
    {
        LifebarParent?.gameObject.SetActive(value);
        Progressbar?.gameObject.SetActive(!value);
    }
    public void RegisterBoss(EntityEnemy boss)
    {
        BossLifebar.AssignEntity(boss.damageable);
    }
    #endregion*/
}
