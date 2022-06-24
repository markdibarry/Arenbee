namespace Arenbee.Framework.ActionEffects
{
    public class ActionEffectDBNull : IActionEffectDB
    {
        public IActionEffect GetEffect(ActionEffectType type) => null;
    }
}