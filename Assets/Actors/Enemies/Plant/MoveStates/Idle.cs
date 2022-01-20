using Arenbee.Framework;

namespace Arenbee.Assets.Actors.Enemies.PlantNS.BaseStates
{
    public class Idle : State<Plant>
    {
        public Idle() { AnimationName = "Idle"; }
        public override void Enter()
        {
            StateMachine.PlayAnimation(AnimationName);
        }

        public override void Update(float delta)
        {
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}