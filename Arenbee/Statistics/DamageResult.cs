using System.Collections.Generic;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public class DamageResult : BaseDamageResult
{
    public ActionType ActionType { get; set; }
    public ElementType ElementDamage { get; set; }
    public StatusEffectType StatusEffectDamage { get; set; }
    public int ElementMultiplier { get; set; }
    public List<Modifier> StatusEffects { get; set; } = new();
}
