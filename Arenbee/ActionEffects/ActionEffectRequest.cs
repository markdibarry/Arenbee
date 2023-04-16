using Arenbee.Actors;
using GameCore.ActionEffects;

namespace Arenbee.ActionEffects;

public class ActionEffectRequest : AActionEffectRequest
{
    public ActionEffectRequest(ActorBody? user, Actor[] targets, int actionType, int value1, int value2)
        : base(actionType, value1, value2)
    {
        User = user;
        Targets = targets;
    }

    public override ActorBody? User { get; }
    public override Actor[] Targets { get; }
}
