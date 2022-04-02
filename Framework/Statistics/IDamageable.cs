using Godot;

namespace Arenbee.Framework.Statistics
{
    public interface IDamageable
    {
        Stats Stats { get; }
        StringName Name { get; }
    }
}