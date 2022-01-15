using Arenbee.Framework;

namespace Arenbee.Assets.Enemies.PlantNS.MoveStates
{
    public class Idle : State<Plant>
    {
        public override void Enter()
        {
            StateController.PlayBase("Idle");
        }

        public override void Update()
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