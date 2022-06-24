using System.Collections.Generic;
using Arenbee.Framework.ActionEffects;

namespace Arenbee.Assets.ActionEffects
{
    public class ActionEffectDB : IActionEffectDB
    {
        public ActionEffectDB()
        {
            _effects = new Dictionary<ActionEffectType, IActionEffect>();
            BuildDB();
        }

        private readonly Dictionary<ActionEffectType, IActionEffect> _effects;

        public IActionEffect GetEffect(ActionEffectType type)
        {
            if (_effects.TryGetValue(type, out var effect))
                return effect;
            return null;
        }

        public void BuildDB()
        {
            _effects[ActionEffectType.RestoreHP] = new RestoreHP();
        }
    }
}