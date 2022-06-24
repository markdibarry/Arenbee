namespace Arenbee.Framework.ActionEffects
{
    public interface IActionEffectDB
    {
        IActionEffect GetEffect(ActionEffectType type);
    }
}