using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Variable", menuName = "Dialogue/Components/Change Variable")]
public class ScriptableDialogueVariableChange : ScriptableDialogue
{
    public Variables.Change[] Changes;
    public override void OnConclude(DialogueUIController DC)
    {
            PlayerController.main.GetGlobalScope().Apply(Changes);
       
    }
    public override IEnumerator Run(DialogueUIController DC)
    {
        OnConclude(DC);
        yield return null;
    }
}
