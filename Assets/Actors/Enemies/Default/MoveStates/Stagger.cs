using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Stagger : State<Actor>
    {
        float _staggerTimer = 0.5f;
        bool _isStaggered = true;
        public override void Enter()
        {
            StateController.ActionStateMachine.TransitionTo(new None());
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