using System.Collections.Generic;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectDB : IStatusEffectDB
    {
        public StatusEffectDB()
        {
            _effects = new Dictionary<StatusEffectType, StatusEffectData>();
            BuildDB();
        }

        private readonly Dictionary<StatusEffectType, StatusEffectData> _effects;

        public StatusEffectData GetEffectData(StatusEffectType type)
        {
            if (_effects.TryGetValue(type, out var effect))
                return effect;
            return null;
        }

        private void BuildDB()
        {
            _effects.Add(StatusEffectType.Burn, new Burn());
            _effects.Add(StatusEffectType.Freeze, new Freeze());
            _effects.Add(StatusEffectType.Paralysis, new Paralyze());
            _effects.Add(StatusEffectType.Poison, new Poison());
            _effects.Add(StatusEffectType.Attack, new Attack());
        }
    }
}