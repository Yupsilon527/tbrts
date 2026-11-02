using System.Collections.Generic;
using System.Linq;

public class AnimationController : Initializable
{
    public HashSet<CountdownAction> animations = new();
    private void Update()
    {
        if (animations.Count > 0)
        {
            foreach (var anim in animations.ToArray())
            {
                if (anim.ShouldFire())
                {
                    anim.Stop(true);
                    animations.Remove(anim);
                }
            }
        }
    }
}
