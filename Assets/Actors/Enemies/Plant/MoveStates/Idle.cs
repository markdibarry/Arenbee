using Arenbee.Framework;

namespace Arenbee.Assets.Enemies.PlantNS.MoveStates
{
    public class Idle : State<Plant>
    {
        public override void Enter()
        {
            AnimationName = "Idle";
            StateController.PlayBase(AnimationName);
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