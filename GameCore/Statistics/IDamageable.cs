using Godot;

namespace GameCore.Statistics;

public interface IDamageable
{
    Stats Stats { get; }
    StringName Name { get; }
}
