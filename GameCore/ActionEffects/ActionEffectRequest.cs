using Arenbee.Statistics;
using GameCore.Actors;

namespace GameCore.ActionEffects;

public class ActionEffectRequest
{
    public AActorBody? User { get; set; }
    public ActionType ActionType { get; set; }
    public int Value1 { get; set; }
    public int Value2 { get; set; }
}
