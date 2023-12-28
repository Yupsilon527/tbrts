
using UnityEngine.Events;

public class EventConditionalActionTrigger  : ConditionalActionTrigger 
{
    public UnityEvent Action;
    public override void TriggerAction()
    {
        Action.Invoke();
        base.TriggerAction();
    }
}
