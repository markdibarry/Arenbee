using System;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public class ConditionTypeDB : IConditionTypeDB
{
    public Type GetConditionType(int conditionType)
    {
        return (ConditionType)conditionType switch
        {
            ConditionType.Timed => typeof(TimedCondition),
            ConditionType.Status => typeof(StatusCondition),
            ConditionType.KO => typeof(KOCondition),
            ConditionType.HPPercent => typeof(HPCondition),
            _ => throw new NotImplementedException()
        };
    }
}
