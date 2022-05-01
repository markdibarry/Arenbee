using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Default.BaseStates
{
    public class Stagger : ActorState
    {
        public Stagger() { AnimationName = "Stagger"; }
        float _staggerTimer = 0.5f;
        bool _isStaggered = true;

        public override void Enter()
        {
            StateController.ActionStateMachine.Reset();
            Actor.IsWalkDisabled = true;
            Actor.IsRunDisabled = true;
            Actor.IsAttackDisabled = true;
            Actor.IsJumpDisabled = true;
            PlayAnimation(AnimationName, true);
        }

        public override ActorState Update(float delta)
        {
            if (_staggerTimer > 0)
                _staggerTimer -= delta;
            else
                _isStaggered = false;
            return CheckForTransitions();
        }

        public override void Exit()
        {
            Actor.IsWalkDisabled = false;
            Actor.IsRunDisabled = false;
            Actor.IsAttackDisabled = false;
            Actor.IsJumpDisabled = false;
            StateController.AnimationOverride = false;
        }

        public override ActorState CheckForTransitions()
        {
            if (!_isStaggered)
                StateController.ResetMachines();
            return null;
        }
    }
}
