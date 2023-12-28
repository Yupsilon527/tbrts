using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Line", menuName = "Dialogue/Components/Line")]
public class DialogueLineSO : ScriptableDialogue
{

    [Header("Dialoge")]
    public Sprite DialogueImage;
    public string DialogueSpeaker;
    public string DialogueQuestion;

    public virtual string GetDialogueLine()
    {
        return DialogueQuestion;
    }
    public IEnumerator LoadDialogue(DialogueUIController DC)
    {
        if (DialogueImage != null)
        {
            DC.dialogImage.sprite = DialogueImage;
            DC.dialogImage.color = Color.white;
            DC.dialogImage.enabled = true;
        }
        else
        {
            DC.dialogImage.enabled = false;
        }
        DC.ChangeCharacterName(DialogueSpeaker);
        string DialogueQuestion = GetDialogueLine();
        yield return DC.TypeDialog(DialogueQuestion,false);
    }
    public override IEnumerator Run(DialogueUIController DC)
    {
        yield return LoadDialogue(DC);
        yield return DC.PostLineWait();
        OnConclude(DC);
    }
    public override void OnConclude(DialogueUIController DC)
    {
        base.OnConclude(DC);
        //DC.dialogImage.enabled = false;
    }
}
