using GameCore.Actors;
using GameCore.Utility;

namespace Arenbee.Actors.Enemies.Default.State;

public class AirStateMachine : AirStateMachineBase
{
    public AirStateMachine(ActorBase actor)
        : base(
            new AirState[]
            {
                new Grounded(actor),
                new Falling(actor)
            },
            actor)
    {
    }

    public class Grounded : AirState
    {
        public Grounded(ActorBase actor) : base(actor)
        {
        }

        public override void Enter()
        {
            Actor.VelocityY = (float)Actor.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override void Update(double delta)
        {
        }

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (!Actor.IsOnFloor())
                return stateMachine.TrySwitchTo<Falling>();
            return false;
        }
    }

    public class Falling : AirState
    {
        public Falling(ActorBase actor) : base(actor)
        {
        }

        public override void Enter() { }

        public override void Update(double delta)
        {
            Actor.ApplyFallGravity(delta);

        }

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Actor.IsOnFloor())
                return stateMachine.TrySwitchTo<Grounded>();
            return false;
        }
    }
}
