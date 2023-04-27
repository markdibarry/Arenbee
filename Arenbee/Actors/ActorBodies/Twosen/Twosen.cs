using Arenbee.Actors.State.DefaultPlayer;
using GameCore.Utility;

namespace Arenbee.Actors.ActorBodies;

public partial class Twosen : ActorBody
{
    public Twosen() : base()
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
