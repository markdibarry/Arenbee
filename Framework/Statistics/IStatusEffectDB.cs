namespace Arenbee.Framework.Statistics
{
    public interface IStatusEffectDB
    {
        StatusEffectData GetEffectData(StatusEffectType type);
    }
}