using GameCore.Actors;

namespace GameCore.ActionEffects;

public interface IActionEffect
{
    bool CanUse(ActionEffectRequest request, ActorBase[] targets);
    void Use(ActionEffectRequest request, ActorBase[] targets);
}

public enum ActionEffectType
{
    None,
    RestoreHP
}
