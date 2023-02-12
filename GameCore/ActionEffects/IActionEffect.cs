using GameCore.Actors;

namespace GameCore.ActionEffects;

public interface IActionEffect
{
    bool CanUse(ActionEffectRequest request, AActor[] targets);
    void Use(ActionEffectRequest request, AActor[] targets);
}

public enum ActionEffectType
{
    None,
    RestoreHP
}
