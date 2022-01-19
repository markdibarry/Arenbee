using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Stagger : State<Actor>
    {
        float _staggerTimer = 0.4f;
        bool _isStaggered = true;

        public override void Enter()
        {
            AnimationName = "Stagger";
            Actor.IsWalkDisabled = true;
            Actor.IsAttackDisabled = true;
            Actor.IsJumpDisabled = true;
            StateController.PlayBase(AnimationName);
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            if (_staggerTimer > 0)
                _staggerTimer -= delta;
            else
                _isStaggered = false;
        }

        public override void Exit()
        {
            Actor.IsWalkDisabled = false;
            Actor.IsAttackDisabled = false;
            Actor.IsJumpDisabled = false;
        }

        public override void CheckForTransitions()
        {
            if (!_isStaggered)
            {
                StateController.ResetMachines();
            }
        }
    }
}
