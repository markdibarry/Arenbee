using System.Collections.Generic;
using Arenbee.Actors.ActorBodies;
using GameCore.Actors;

namespace Arenbee.Actors;

public class ActorBodyDB : AActorBodyDB
{
    public override IReadOnlyDictionary<string, string> ActorBodies { get; } = new Dictionary<string, string>()
    {
        { ActorBodyIds.Twosen, Twosen.GetScenePath() },
        { ActorBodyIds.Ball, Ball.GetScenePath() },
        { ActorBodyIds.Whisp, Whisp.GetScenePath() },
        { ActorBodyIds.Orc, Orc.GetScenePath() },
        { ActorBodyIds.Plant, Plant.GetScenePath() }
    };
}
