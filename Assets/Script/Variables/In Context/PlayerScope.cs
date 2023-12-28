using System.Collections;
using System.Collections.Generic;
using Variables;

public class PlayerScope : VariableScope
{
    public override void ChangeVariable(string vName, Change.Case change, float nValue)
    {
        string vNameLower = vName.ToLower();
        foreach (Achievment a in goals)
        {
            if (a.Condition.variableName == vNameLower && a is IncrementalAchievment ia)
            {
                ia.StoredValue.Change(change, nValue);
                CheckAchievementCompleted(a);
            }
        }
        base.ChangeVariable(vNameLower, change, nValue);
    }
    #region Achievments
    public class Achievment
    {
        public string InternalName;
        public Condition Condition;

        bool achieved = false;
        public void SetCompleted(bool value)
        {
            achieved = value;
        }
        public bool GetCompleted()
        {
            return achieved;
        }
    }
    public class IncrementalAchievment : Achievment
    {
        public Variable StoredValue;
    }
    protected List<Achievment> goals = new  List<Achievment>();

    public bool HasCompletedAchievment(string achievmentName)
    {
        foreach (Achievment a in goals)
        {
            if (a.InternalName == achievmentName)
            {
                return HasCompletedAchievment(a);
            }
        }
        return false;
    }
    public bool HasCompletedAchievment(Achievment a)
    {
        if (a is IncrementalAchievment ia)
        {
            return a.Condition.ConditionMet(ia.StoredValue.GetFloatValue());
        }
            return ConditionMet(a.Condition);
        
    }
    public virtual void CheckAchievementCompleted(Achievment achievment)
    {
        bool completed = HasCompletedAchievment(achievment);
 //       if (completed && !achievment.GetCompleted())
//  TODO          HubworldUIManager.main.NotificationPopup.DisplayNotification(NotificationPopup.NotificationType.achievment_complete, achievment.InternalName);
        achievment.SetCompleted(completed);
    }
    #endregion
}
