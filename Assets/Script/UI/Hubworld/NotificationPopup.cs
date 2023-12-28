using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationPopup : MonoBehaviour
{
    public float DefaultNotificationDuration = 5;
    public RectTransform transform;
    public TMPro.TextMeshProUGUI notificationText;

    public enum NotificationType
    {
        quest_new,
        quest_complete,
        achievment_complete,
        level_up,   //TODO
    }
    public void DisplayNotification(NotificationType notification, string notificationData)
    {
        switch (notification)
        {
            case NotificationType.quest_new:
                notificationText.text = $"New Quest!\n{notificationData}";
                break;
            case NotificationType.quest_complete:
                notificationText.text = $"Quest Completed!\n{notificationData}";
                break;
            case NotificationType.achievment_complete:
                notificationText.text = $"Achievment Completed!\n{notificationData}";
                break;
        }
        StartPopinAnimation(DefaultNotificationDuration);
    }
    Coroutine animatioCoroutine;
    void StartPopinAnimation(float duration)
    {
        if (animatioCoroutine != null)
        {
            StopCoroutine(animatioCoroutine);
        }

        bool hasIntro = gameObject.activeSelf;
        gameObject.SetActive(true);
        StartCoroutine(DisplayNotificationCoroutine(hasIntro, duration));
    }
    IEnumerator DisplayNotificationCoroutine(bool hasIntro, float duration)
    {
        if (hasIntro)
        {
            yield return PopInCoroutine();
        }
        yield return new WaitForSeconds(duration);
        yield return PopOutCoroutine();
    }
    IEnumerator PopInCoroutine()
    {
        yield return new WaitForEndOfFrame();
    }
    IEnumerator PopOutCoroutine()
    {
        yield return new WaitForEndOfFrame();
        animatioCoroutine = null;
        gameObject.SetActive(false);
    }
}
