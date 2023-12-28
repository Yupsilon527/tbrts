using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variables
{
    [System.Serializable]
    public class Change
    {
        public enum Case
        {
            set,
            add,
            substract,
            min,
            max,
            multiply,
            divide
        }
        public string variableName;
        public float value;
        public Case change;
    }
    public class Variable
    {
        bool incremental = false;
        float Value;
        public Variable(float value, bool i)
        {
            SetFloatValue(value);
            incremental = i;
        }
        public Variable(float value)
        {
            SetFloatValue(value);
        }
        public Variable(bool value)
        {
            SetBoolValue(value);
        }
        public void SetBoolValue(bool nValue)
        {
            Value = nValue ? 1 : 0;
        }
        public bool GetBoolValue()
        {
            return Value > 0;
        }
        public void SetFloatValue(float nValue)
        {
            Value = nValue;
        }
        public float GetFloatValue()
        {
            return Value;
        }
        public void Change(Change.Case change, float value)
        {
            switch (change)
            {
                case Variables.Change.Case.set:
                    if (!incremental)
                        Value = value;
                    break;
                case Variables.Change.Case.min:
                    if (!incremental)
                        Value = Mathf.Min(Value, value);
                    break;
                case Variables.Change.Case.max:
                    if (!incremental)
                        Value = Mathf.Max(Value, value);
                    break;
                case Variables.Change.Case.add:
                    Value += value;
                    break;
                case Variables.Change.Case.substract:
                    Value -= value;
                    break;
                case Variables.Change.Case.multiply:
                    Value *= value;
                    break;
                case Variables.Change.Case.divide:
                    Value /= value;
                    break;
            }
        }
    }
    [System.Serializable]
    public class Condition
    {
        public enum Check
        {
            equal,
            less,
            lesseq,
            greater,
            greatereg,
            boolean,
        }
        public string variableName;
        public float value;
        public Check check;
        public bool ConditionMet(float original)
        {
            switch (check)
            {
                case Condition.Check.equal:
                    return original == value;
                case Condition.Check.boolean:
                    return original > 0;
                case Condition.Check.less:
                    return original < value;
                case Condition.Check.lesseq:
                    return original <= value;
                case Condition.Check.greater:
                    return original > value;
                case Condition.Check.greatereg:
                    return original >= value;
            }
            return true;
        }
    }
    public class VariableScope
    {
       
        #region Store Variables
        Dictionary<string, Variable> scope = new Dictionary<string, Variable>();
        public bool HasVariable(string vName)
        {
            return (scope.ContainsKey(vName.ToLower()));
        }
        public Variable GetVariable(string vName)
        {
            vName = vName.ToLower();
            if (scope.ContainsKey(vName))
            {
                return scope[vName];
            }
            return SetVariable(vName, 0);
        }

        public Variable SetVariable(string vName, float nValue)
        {
            vName = vName.ToLower();
            Debug.Log("Set variable " + vName + " to " + nValue);
            if (scope.ContainsKey(vName))
            {
                scope[vName].SetFloatValue(nValue);
            }
            else
            {
                scope.Add(vName, new Variable(nValue));
            }
            OnVariableChange(vName);
            return scope[vName];
        }
        public Variable SetVariable(string vName, bool nValue)
        {
            vName = vName.ToLower();
            Debug.Log("Set variable " + vName + " to " + nValue);
            if (scope.ContainsKey(vName))
            {
                scope[vName].SetBoolValue(nValue);
            }
            else
            {
                scope.Add(vName, new Variable(nValue));
            }
            OnVariableChange(vName);
            return scope[vName];
        }
        protected virtual void OnVariableChange(string variable)
        {
            foreach (ConditionalActionTrigger  vrc in LevelController.main.Reactors)
            {
                CheckVars(vrc, variable);
            }
        }
        void CheckVars(ConditionalActionTrigger  vrc, string variable)
        {
            bool conditionMet = true;

            Debug.Log("REACTORCHEC " + vrc.name + "WITH " + vrc.Conditions.Length + " CONDITIONS");
            if (vrc.Conditions != null && vrc.Conditions.Length > 0)
            {
                bool tracksVariable = false;

                foreach (Condition c in vrc.Conditions)
                {
                    string lowerName = c.variableName.ToLower();
                    Debug.Log($"CONDITIONCHECK {lowerName} NEEDS TO BE {c.check} {c.value}");
                    if (lowerName == variable)
                        tracksVariable = true;
                    if (!ConditionMet(c))
                    {
                        Debug.Log("CONDITIONCHECK FAILED " + lowerName);
                        conditionMet = false;
                        break;
                    }
                    Debug.Log("CONDITIONCHECK PASS " + lowerName);
                }
                if (tracksVariable && conditionMet)
                {
                    vrc.PerformAction();
                }
            }
        }
        public void Apply(VariableScope temp)
        {
            if (temp == null)
                return;
            foreach (KeyValuePair<string, Variable> num in temp.scope)
            {
                SetVariable(num.Key, num.Value.GetFloatValue());
            }
        }
        #endregion
        #region Set Variables
        public void Apply(Change[] changes)
        {
            if (changes == null || changes.Length == 0)
                return;
            foreach (Change num in changes)
            {
                Apply(num);
            }
        }
        public void Apply(Change change)
        {
            ChangeVariable(change.variableName.ToLower(), change.change, change.value);
        }
        public virtual void ChangeVariable(string variableName, Change.Case change, float value)
        {
            string vNameLower = variableName.ToLower();
            Debug.Log($"[Variable] Change variable { vNameLower} to {change} {value}");
            GetVariable(vNameLower).Change(change, value);
            OnVariableChange(vNameLower);
        }
        #endregion
        #region Conditions
        public virtual bool ConditionMet(Condition c)
        {
            return c.ConditionMet(GetVariable(c.variableName).GetFloatValue());
        }
        public bool AllConditionsMet(Condition[] cs)
        {
            if (cs != null && cs.Length > 0)
                foreach (Condition c in cs)
                {
                    if (!ConditionMet(c))
                        return false;
                }
            return true;
        }
        #endregion
        public void ClearVars()
        {
            scope.Clear();
        }
    }
}