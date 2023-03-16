using GameCore.Enums;
using Godot;

namespace GameCore.Statistics;

public partial class Condition : Resource
{
    public Condition()
    {
    }

    public Condition(Condition condition)
        : this(conditionType: condition.ConditionType,
              target: condition.Target,
              startValue: condition.StartValue,
              currentValue: condition.CurrentValue,
              targetValue: condition.TargetValue,
              logicOp: condition.LogicOp)
    {
    }

    public Condition(int conditionType, int target, int startValue, int currentValue, int targetValue, LogicOp logicOp)
    {
        ConditionType = conditionType;
        Target = target;
        StartValue = startValue;
        CurrentValue = currentValue;
        TargetValue = targetValue;
        LogicOp = logicOp;
    }

    [Export] public int ConditionType { get; set; }
    [Export] public int Target { get; set; }
    [Export] public int StartValue { get; set; }
    [Export] public int CurrentValue { get; set; }
    [Export] public CompareOp CompareOp { get; set; }
    [Export] public int TargetValue { get; set; }
    [Export] public LogicOp LogicOp { get; set; }
    public ConditionEventFilter? EventFilter { get; set; }

    public virtual void Reset()
    {
        CurrentValue = StartValue;
    }
}
