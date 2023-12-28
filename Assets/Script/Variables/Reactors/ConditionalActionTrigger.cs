
using UnityEngine;

public class ConditionalActionTrigger  : MonoBehaviour
{
    public Variables.Condition[] Conditions;
    public bool triggerOnce = false;
    private void Start()
    {
        if (LevelController.main != null)
            LevelController.main.RegisterReactor(this);
    }
    bool hasTriggered = false;
    public virtual void TriggerAction()
    {
        hasTriggered = true;
    }
    public void PerformAction()
    {
        if (!hasTriggered || !triggerOnce)
        {
            TriggerAction();
        }
    }
}
