using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room Transition", menuName = "Dialogue/Components/Room Transition")]
public class ScriptableDialogueRoomTransition : ScriptableDialogue
{
    public string RoomName = "";
    public Vector2 RoomPoint = Vector2.zero;
    public bool Instant = false;
    public override void OnConclude(DialogueUIController DC)
    {
        //PlayerTransitionController.main.TransitionRoom(RoomName, RoomPoint, SetOrientation, Instant);
    }
    public override IEnumerator Run(DialogueUIController DC)
    {
        OnConclude(DC);
        yield return base.Run(DC);
    }
}
