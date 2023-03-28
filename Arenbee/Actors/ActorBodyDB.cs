using System.Collections.Generic;
using Arenbee.Actors.Enemies;
using Arenbee.Actors.Players;

namespace Arenbee.Actors;

public static class ActorBodyDB
{
    public static IReadOnlyDictionary<string, string> ActorBodies { get; } = new Dictionary<string, string>()
    {
        { ActorBodyIds.Twosen, Twosen.GetScenePath() },
        { ActorBodyIds.Ball, Ball.GetScenePath() },
        { ActorBodyIds.Whisp, Whisp.GetScenePath() },
        { ActorBodyIds.Orc, Orc.GetScenePath() },
        { ActorBodyIds.Plant, Plant.GetScenePath() }
    };

    public static string? ById(string bodyId)
    {
        if (ActorBodies.TryGetValue(bodyId, out string? result))
            return result;
        return default;
    }
}
