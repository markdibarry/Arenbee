using GameCore.Actors;

namespace Arenbee.Actors.State.DefaultEnemy;

public class AirStateMachine : AirStateMachineBase
{
    public AirStateMachine(ActorBody actor)
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
        public Grounded(ActorBody actor) : base(actor)
        {
        }

        public override void Enter()
        {
            ActorBody.VelocityY = (float)ActorBody.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override void Update(double delta)
        {
        }

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (!ActorBody.IsOnFloor())
                return stateMachine.TrySwitchTo<Falling>();
            return false;
        }
    }

    public class Falling : AirState
    {
        public Falling(ActorBody actor) : base(actor)
        {
        }

        public override void Enter() { }

        public override void Update(double delta)
        {
            ActorBody.ApplyFallGravity(delta);

        }

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (ActorBody.IsOnFloor())
                return stateMachine.TrySwitchTo<Grounded>();
            return false;
        }
    }
}
