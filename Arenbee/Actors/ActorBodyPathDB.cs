using System.Collections.Generic;
using Arenbee.Actors.ActorBodies;
using GameCore.Actors;

namespace Arenbee.Actors;

public class ActorBodyPathDB : IActorBodyPathDB
{
    public IReadOnlyDictionary<string, string> ActorBodies { get; } = new Dictionary<string, string>()
    {
        { ActorBodyIds.Twosen, Twosen.GetScenePath() },
        { ActorBodyIds.Ball, Ball.GetScenePath() },
        { ActorBodyIds.Whisp, Whisp.GetScenePath() },
        { ActorBodyIds.Orc, Orc.GetScenePath() },
        { ActorBodyIds.Plant, Plant.GetScenePath() }
    };

    public string? GetById(string bodyId)
    {
        if (ActorBodies.TryGetValue(bodyId, out string? result))
            return result;
        return null;
    }
}
