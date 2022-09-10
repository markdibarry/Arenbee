using GameCore.Actors;

namespace GameCore.ActionEffects;

public interface IActionEffect
{
    bool CanUse(ActionEffectRequest request, Actor[] targets);
    void Use(ActionEffectRequest request, Actor[] targets);
}

public enum ActionEffectType
{
    None,
    RestoreHP
}
