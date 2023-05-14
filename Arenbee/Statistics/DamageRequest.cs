using System.Collections.Generic;
using GameCore.Statistics;
using System;

namespace Arenbee.Statistics;

public class DamageRequest : BaseDamageRequest
{
    public DamageRequest()
    {
        ActionType = ActionType.Environment;
        ElementType = ElementType.None;
        StatusChances = Array.Empty<StatusChance>();
    }

    public ActionType ActionType { get; set; }
    public ElementType ElementType { get; set; }
    public IReadOnlyCollection<StatusChance> StatusChances { get; set; }
}
