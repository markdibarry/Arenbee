namespace Arenbee.Framework.Statistics
{
    public class StatusEffectDBNull : IStatusEffectDB
    {
        public StatusEffectData GetEffectData(StatusEffectType type)
        {
            return null;
        }

        public StatusEffectData GetEffectData(int type)
        {
            return null;
        }
    }
}