namespace Arenbee.Actors.ActorBodies;

public partial class Whisp : ActorBody
{
    public class AirStateMachine : AirStateMachineBase
    {

        public AirStateMachine(ActorBody actor)
            : base(
                new AirState[]
                {
                    new Floating(actor)
                },
                actor)
        {
        }

        private class Floating : AirState
        {
            public Floating(ActorBody actor) : base(actor)
            {
            }

            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
            }
        }
    }
}
