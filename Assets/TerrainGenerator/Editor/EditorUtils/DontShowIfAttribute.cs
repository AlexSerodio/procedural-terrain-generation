using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DontShowIfAttribute : ShowIfAttribute
{
    public DontShowIfAttribute(ActionOnConditionFail action, ConditionOperator conditionOperator, params string[] conditions)
        : base(action, conditionOperator, conditions) { }
}
