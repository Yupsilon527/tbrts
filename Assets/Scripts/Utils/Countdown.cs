using System;
using UnityEngine;

public class Countdown
{
    float startTime;
    float durationTime;

    public Countdown(float durationTime)
    {
        this.durationTime = durationTime;
    }

    public float GetEndTime()
    {
        return startTime + durationTime;
    }
    public bool IsRunning()
    {
        return Time.time < GetEndTime();
    }
    public float GetTimeRemaining()
    {
        return GetEndTime() - Time.time;
    }
    public float GetDuration()
    {
        return durationTime;
    }
    public float GetTimePercentage()
    {
        return (Time.time - startTime) / durationTime;
    }
    void SetTime(float start, float dur)
    {
        startTime = start;
        durationTime = dur;
    }
    public void Restart(float start)
    {
        startTime = start;
    }
    public void Extend(float dur)
    {
        Extend(Time.time, dur);
    }
    public void Set(float dur)
    {
        SetTime(Time.time, dur);
    }
    public void Unique(float dur)
    {
        if (!IsRunning())
        {
            SetTime(Time.time, dur);
        }
    }
    public void Override(float dur)
    {
        SetTime(Time.time, dur);
    }
    public void Extend(float start, float dur)
    {
        if (GetEndTime() + dur > start + dur)
        {
            durationTime += dur;
        }
        else
        {
            SetTime(start, dur);
        }
    }
    public void Shorten(float dur = 1)
    {
        durationTime -= dur;
    }
    public void Stop()
    {
        startTime = -1;
        durationTime = -1;
    }
}

public class CountdownAction : Countdown
{
    public Action concludeAction;
    public CountdownAction(float durationTime) : base(durationTime)   { }
    public CountdownAction(float durationTime, Action concludeAction) : base(durationTime)   {
        this.concludeAction = concludeAction;
    }

    public bool ShouldFire()
    {
        return !IsRunning() && concludeAction != null;
    }
    public void Start(Action act, float dur = 1)
    {
        concludeAction = act;
        Set(dur);
    }
    public void Stop(bool expire)
    {
        if (expire)
            concludeAction?.Invoke();
        concludeAction = null;
        Stop();
    }
}