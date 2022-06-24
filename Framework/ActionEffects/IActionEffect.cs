using Arenbee.Framework.Actors;

namespace Arenbee.Framework.ActionEffects
{
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
}
