using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimerEventTrigger : MonoBehaviour
{
    public float interval;
    public bool repeating;
    public UnityEvent onEplased;

    public void StartCountdown()
    {
        if (timerC == null)
        {
            StopCooldown();
            timerC= StartCoroutine(Countdown());
        }
    }
    public void StopCooldown()
    {
        if (timerC != null)
        {
            StopCoroutine(timerC);
        }
    }
    Coroutine timerC;
    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(interval);
        onEplased.Invoke();
        if (repeating)
            StartCountdown();
    }
}
