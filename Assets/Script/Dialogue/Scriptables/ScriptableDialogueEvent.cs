using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event", menuName = "Dialogue/Components/Event")]

public class ScriptableDialogueEvent : ScriptableDialogue
{
    public UnityEvent Event;
    public override void OnConclude(DialogueUIController DC)
    {
        Event.Invoke();
    }
    public override IEnumerator Run(DialogueUIController DC)
    {
        OnConclude(DC);
        yield return null;
    }
}
