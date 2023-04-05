using Arenbee.Actors.State.DefaultPlayer;
using GameCore.Extensions;

namespace Arenbee.Actors.ActorBodies;

public partial class Twosen : ActorBody
{
    public Twosen()
    {
        WalkSpeed = 100;
        StateController = new StateController(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            new ActionStateMachine(this));
    }

    public static string GetScenePath() => GDEx.GetScenePath();
}
