using System;
using UnityEngine;

[Serializable]
public class SpriteAnimation
{
    public string name;

    public int priority;
    public float startTime;
    public float duration;
    public int[] frames;
    public int repeats;

    public SpriteAnimation() { }

    public SpriteAnimation(string name, int priority, float duration, int[] frames, int repeats, float startTime = 0f)
    {
        this.name = name;
        this.priority = priority;
        this.duration = duration;
        this.frames = frames;
        this.repeats = repeats;
        this.startTime = startTime;
    }

    public float FrameDuration => (frames == null || frames.Length == 0) ? 0f : duration / frames.Length;

    public int GetFrameSpriteIndex(float time)
    {
        if (frames == null || frames.Length == 0 || duration <= 0f)
            return -1;

        float frameDuration = FrameDuration;
        int frameIndex = Mathf.Clamp(Mathf.FloorToInt(time / frameDuration), 0, frames.Length - 1);
        return frames[frameIndex];
    }
}
