using GameCore.Actors;

namespace Arenbee.Actors.Enemies;

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

            public override void Update(double delta) { }

            public override void Exit() { }
        }
    }
}
