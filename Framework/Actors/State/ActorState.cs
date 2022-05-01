using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Arenbee.Framework.Items;

namespace Arenbee.Framework
{
    public abstract class ActorState
    {
        public ActorInputHandler InputHandler => Actor.InputHandler;
        public string AnimationName { get; set; }
        public bool IsInitialState { get; set; }
        public StateController StateController { get; set; }
        protected Actor Actor { get; private set; }
        protected Weapon Weapon { get; private set; }
        public ActorStateMachine StateMachine { get; set; }

        public virtual void Init()
        {
            Actor = StateMachine.Actor;
            StateController = StateMachine.StateController;
            Weapon = Actor.WeaponSlot.CurrentWeapon;
        }

        public abstract void Enter();
        public abstract ActorState Update(float delta);
        public abstract void Exit();
        public abstract ActorState CheckForTransitions();

        protected void PlayAnimation(string animationName, bool force = false)
        {
            StateMachine.PlayAnimation(this, animationName, force);
        }
    }

    public class None : ActorState
    {
        public None() { IsInitialState = true; }

        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            return null;
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            return null;
        }
    }
}