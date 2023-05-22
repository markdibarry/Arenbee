using System.Collections.Generic;
using GameCore.Statistics;
using System;
using Godot;

namespace Arenbee.Statistics;

public class DamageRequest : IDamageRequest
{
    public DamageRequest()
    {
        SourceName = string.Empty;
        Value = 1;
        ActionType = ActionType.Environment;
        ElementType = ElementType.None;
        StatusChances = Array.Empty<StatusChance>();
    }

    public ActionType ActionType { get; set; }
    public ElementType ElementType { get; set; }
    public IReadOnlyCollection<StatusChance> StatusChances { get; set; }
    public string SourceName { get; set; }
    public Vector2 SourcePosition { get; set; }
    public int Value { get; set; }
}
