using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionalActionTrigger : ConditionalActionTrigger
{
    public enum ActionType
    {
        winGame,
        loseGame,
    }
    public ActionType myAction;
    private void Start()
    {
        switch (myAction)
        {
            case ActionType.winGame:
                action = () => PlayerController.main.party.DungeonSucceed();

                break;
            case ActionType.loseGame:
                action = () => PlayerController.main.party.DungeonFail();
                break;
        }
    }

    Action action;
    public override void TriggerAction()
    {
        action?.Invoke();
        base.TriggerAction();
    }
}
