public static class BuiltInAnimations
{
    public enum BuiltinAnimation
    {
        Walk,
        Hurt,
        Attack,
        Spell,
    }

    public static SpriteAnimation Walk => new SpriteAnimation(
        name: "Walk",
        priority: 2,
        duration: 0.8f,
        frames: new[]
        {
            (int)UnitDefines.SpriteFrame.Idle,
            (int)UnitDefines.SpriteFrame.Walk,
            (int)UnitDefines.SpriteFrame.Idle,
            (int)UnitDefines.SpriteFrame.Walk
        },
        repeats: -1
    );

    public static SpriteAnimation Hurt => new SpriteAnimation(
        name: "Hurt",
        priority: 5,
        duration: 0.3f,
        frames: new[] { (int)UnitDefines.SpriteFrame.Hurt },
        repeats: 0 // plays once
    );
    public static SpriteAnimation Attack => new SpriteAnimation(
        name: "Attack",
        priority: 4,
        duration: 0.5f,
        frames: new[] { (int)UnitDefines.SpriteFrame.Attack,(int)UnitDefines.SpriteFrame.IdleReady },
        repeats: 0 // plays once
    );
    public static SpriteAnimation Spell => new SpriteAnimation(
        name: "Attack",
        priority: 4,
        duration: 0.5f,
        frames: new[] { (int)UnitDefines.SpriteFrame.IdleReady, (int)UnitDefines.SpriteFrame.Spell,(int)UnitDefines.SpriteFrame.IdleReady },
        repeats: 0 // plays once
    );

}