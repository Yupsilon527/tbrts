using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueUIController : Window
{
    [Header("Components")]

    public int wordsPerSecond = 10;

    [Header("Dialogue Box")]
    public GameObject DialogueBox;
    public GameObject ExpositionBox;

    public Image dialogImage;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI expositionText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI skipText;

    public void SetSkipping(bool Value)
    {
        Skipping = Value;
    }

    Coroutine CutsceneCoroutine;
    private void Update()
    {
        HandleExit();
    }
    bool Skipping = false;
    float skipPercent = 0;
    void HandleExit()
    {
        if (CutsceneCoroutine!=null) //TODO define skip key
        {
            if (Skipping)
            {
                skipPercent += Time.deltaTime * 2;
                if (skipText!=null)
                skipText.color = new Color(1, 1, 1, skipPercent);
            }
            else
            {
                skipPercent = 0;
                if (skipText != null)
                    skipText.color = Color.clear;
            }
        }
    }
    public void PlayCutscene(DialogueScriptSO Script)
    {
        //TODO    HubworldUIManager.main.windowManager.OpenWindow(HubworldUIManager.main.DialogueWindow);
        
        EndDialogue();

        CutsceneCoroutine = StartCoroutine(PlayCutsceneCoroutine(Script));
    }
    public bool IsInDialogueMode()
    {
        return gameObject.activeSelf && (CutsceneCoroutine != null );
    }
    DialogueScriptSO mDialogue;
    int quote;
    IEnumerator PlayCutsceneCoroutine(DialogueScriptSO Script)
    {
        HideDialogue();
        skipPercent = 0;
        mDialogue = Script;
        quote = 0;
        while (quote < mDialogue?.Dialogue?.Length)
        {
            if (Script.Dialogue[quote] != null)
            {
                if (skipPercent >= 1)
                {
                    Script.Dialogue[quote].OnConclude(this);
                }
                else
                {
                    yield return Script.Dialogue[quote].Run(this);
                }
            }
            quote++;
        }
        Close();
    }
    public void SkipDialogue()
    {
        while (mDialogue != null && quote < mDialogue.Dialogue.Length)
            {
                quote++;
                mDialogue.Dialogue[quote-1].OnConclude(this);
            }
        mDialogue = null;
    }
    public void HandlePlayerSkip()
    {
        SkipLine = true;
    }
    public void ClearDialogue()
    {
    }
    public void EndDialogue()
    {
        if (CutsceneCoroutine != null)
            StopCoroutine(CutsceneCoroutine);
        CutsceneCoroutine = null;
        mDialogue = null;
    }
    public override void Close()
    {
        EndDialogue();
        base.Close();
    }

    bool SkipLine = false;
    public IEnumerator PostLineWait()
    {
            yield return SkippableWait();
        
    }

    public void HideDialogue()
    {
        dialogImage.enabled = false;
        EnableDisableExposition(false);
        EnableDisableDialogue(false);
    }
    void EnableDisableDialogue(bool value)
    {
        DialogueBox.SetActive(value);
    }
    void EnableDisableExposition(bool value)
    {
        ExpositionBox.SetActive( value);
    }

    public IEnumerator TypeDialog(string dialog, bool exposition)
    {

        if (exposition)
        {
            EnableDisableExposition(true);
            EnableDisableDialogue(false);
            expositionText.text = "";
        }
        else
        {
            EnableDisableExposition(false);
            EnableDisableDialogue(true);
            dialogText.text = "";
        }

        char[] line = dialog.ToCharArray();

        for (int iC = 0; iC < line.Length; iC++)
        {
            if (line[iC] != ' ')
            {
                bool writeWord = true;
                while (iC < line.Length && writeWord)
                {
                    if (exposition)
                    {
                        expositionText.text += line[iC];
                    }
                    else
                    {
                        dialogText.text += line[iC];
                    }
                    iC++;
                    if (iC < line.Length && line[iC] == ' ')
                    {
                        if (exposition)
                        {
                            expositionText.text += line[iC];
                        }
                        else
                        {
                            dialogText.text += line[iC];
                        }
                        writeWord = false;
                    }
                }
            }

            if (SkipLine || skipPercent>=1)
                break;
            yield return new WaitForSeconds(1f / wordsPerSecond);
        }
        if (exposition)
        {
            expositionText.text = dialog;
        }
        else
        {
            dialogText.text = dialog;
        }
    }
    public float GetWaitValue(string line)
    {
        return 2f + line.Length * .03f; 
    }
    IEnumerator SkippableWait()
    {
        SkipLine = false;
        while (!SkipLine && skipPercent < 1)
        {
            yield return new WaitForFixedUpdate();
        }
        SkipLine = false;
    }

    public void ChangeCharacterName(string CharacterName)
    {
            if (nameText != null)
                nameText.text = CharacterName;
        
    }

    public AudioSource audiosrc;
    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (audiosrc != null)
        {
            audiosrc.pitch = pitch;
            audiosrc.PlayOneShot(clip, volume);
        }
    }
}
