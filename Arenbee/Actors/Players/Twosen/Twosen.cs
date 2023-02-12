using Arenbee.Actors.Default.State;
using GameCore.Actors;
using GameCore.Extensions;

namespace Arenbee.Actors.Players;

public partial class Twosen : ActorBody
{
    public Twosen()
    {
        WalkSpeed = 100;
        StateController = new AStateController(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            new ActionStateMachine(this));
    }

    public static string GetScenePath() => GDEx.GetScenePath();
}
