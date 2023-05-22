using System.Collections.Generic;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public class DamageResult : IDamageResult
{
    public ActionType ActionType { get; set; }
    public ElementType ElementDamage { get; set; }
    public StatusEffectType StatusEffectDamage { get; set; }
    public int ElementMultiplier { get; set; }
    public List<Modifier> StatusEffects { get; set; } = new();
    public string RecieverName { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public Vector2 SourcePosition { get; set; }
    public int TotalDamage { get; set; }
}
