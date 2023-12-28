using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variables;

namespace ContextVars
{
    public enum Scope
    {
        global,
        dungeon,
    }
    [System.Serializable]
    public class ConditionContext : Condition
{
    public Scope context;
    public bool ConditionMet()
    {
        switch (context)//TODO nullchecks
            {
            case Scope.global:
                return PlayerController.main.GetGlobalScope().ConditionMet(this);
            case Scope.dungeon:
                return PlayerController.main.GetDungeonScope().ConditionMet(this);
        }
        return false;
    }
}
[System.Serializable]
public class ChangeContext : Change
    {
        public Scope context;
        public ChangeContext(string variableName, float value, Case change, Scope context)
        {
            this.variableName = variableName;
            this.value = value;
            this.change = change;
            this.context = context;
        }
        public void Apply()
        {
            switch (context)
            {
                case Scope.global:
                    PlayerController.main.GetGlobalScope().Apply(this);
                    break;
                case Scope.dungeon:
                     PlayerController.main.GetDungeonScope().Apply(this);
                    break;
            }
        }
    }

}