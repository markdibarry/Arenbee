using GameCore.Actors;

namespace Arenbee.Actors.Enemies;

public partial class Whisp : Actor
{
    public class AirStateMachine : AirStateMachineBase
    {

        public AirStateMachine(ActorBase actor)
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
            public Floating(ActorBase actor) : base(actor)
            {
            }

            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
            }

            public override void Update(double delta) { }

            public override void Exit() { }
        }
    }
}
