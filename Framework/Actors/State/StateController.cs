using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class StateController
    {
        public StateController(Actor actor)
        {
            _actor = actor;
            BaseStateMachine = new StateMachine(_actor, this, StateMachineType.Base);
            JumpStateMachine = new StateMachine(_actor, this, StateMachineType.Jump);
            ActionStateMachine = new StateMachine(_actor, this, StateMachineType.Action);
            CreateStateDisplay();
        }

        public IStateMachine JumpStateMachine { get; }
        public IStateMachine BaseStateMachine { get; }
        public IStateMachine ActionStateMachine { get; }
        private readonly Actor _actor;
        public AnimationPlayer ActorAnimationPlayer
        {
            get { return _actor.AnimationPlayer; }
        }
        public Weapon CurrentWeapon
        {
            get { return _actor.WeaponSlot.CurrentWeapon; }
        }
        public AnimationPlayer WeaponAnimationPlayer
        {
            get { return CurrentWeapon?.AnimationPlayer; }
        }
        private Label _jumpStateDisplay;
        private Label _moveStateDisplay;
        private Label _actionStateDisplay;

        public void Init(IState baseState, IState jumpState, IState actionState)
        {
            BaseStateMachine.Init(baseState);
            JumpStateMachine.Init(jumpState);
            if (CurrentWeapon == null)
                ActionStateMachine.Init(actionState);
        }

        public void ResetMachines()
        {
            BaseStateMachine.TransitionTo(BaseStateMachine.InitialState);
            JumpStateMachine.TransitionTo(JumpStateMachine.InitialState);
            ActionStateMachine.TransitionTo(ActionStateMachine.InitialState);
        }

        public void PlayAnimation(string animationName, StateMachineType stateMachineType)
        {
            switch (stateMachineType)
            {
                case StateMachineType.Action:
                    PlayActionAnimation(animationName);
                    break;
                case StateMachineType.Jump:
                    PlayJumpAnimation(animationName);
                    break;
                case StateMachineType.Base:
                    PlayBaseAnimation(animationName);
                    break;
            }
        }

        public void PlayActionAnimation(string animationName)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.SetHitBoxAction();
                ActorAnimationPlayer.Stop();
                PlayIfAvailable(CurrentWeapon.WeaponTypeName + animationName);
                CurrentWeapon.AnimationPlayer.Stop();
                CurrentWeapon.AnimationPlayer.Play(animationName);
            }
            else
            {
                PlayIfAvailable(animationName);
            }
        }

        public void PlayJumpAnimation(string animationName)
        {
            if (string.IsNullOrEmpty(ActionStateMachine.State.AnimationName))
            {
                PlayIfAvailable(animationName);
            }
        }

        public void PlayBaseAnimation(string animationName)
        {
            if (string.IsNullOrEmpty(ActionStateMachine.State.AnimationName)
                && string.IsNullOrEmpty(JumpStateMachine.State.AnimationName))
            {
                PlayIfAvailable(animationName);
            }
        }

        // public void PlaySubWeaponAttack(string animationName)
        // {

        // }

        public void PlayIfAvailable(string animationName)
        {
            if (ActorAnimationPlayer.HasAnimation(animationName))
            {
                ActorAnimationPlayer.Play(animationName);
            }
        }

        public void PlayFallbackAnimation()
        {
            string animation = null;
            if (!string.IsNullOrEmpty(ActionStateMachine.State.AnimationName))
            {
                animation = ActionStateMachine.State.AnimationName;
            }
            else if (!string.IsNullOrEmpty(JumpStateMachine.State.AnimationName))
            {
                animation = JumpStateMachine.State.AnimationName;
            }
            else if (!string.IsNullOrEmpty(BaseStateMachine.State.AnimationName))
            {
                animation = BaseStateMachine.State.AnimationName;
            }
            if (animation != null) PlayIfAvailable(animation);
        }

        public void UpdateStates(float delta)
        {
            BaseStateMachine.Update(delta);
            JumpStateMachine.Update(delta);
            ActionStateMachine.Update(delta);
            _moveStateDisplay.Text = BaseStateMachine.State.GetType().Name;
            _jumpStateDisplay.Text = JumpStateMachine.State.GetType().Name;
            _actionStateDisplay.Text = ActionStateMachine.State.GetType().Name;
        }

        private void CreateStateDisplay()
        {
            //Debug Only
            var stateDisplay = GD.Load<PackedScene>(PathConstants.StateDisplay).Instantiate<Control>();
            _jumpStateDisplay = stateDisplay.GetNode<Label>("JumpState");
            _moveStateDisplay = stateDisplay.GetNode<Label>("MoveState");
            _actionStateDisplay = stateDisplay.GetNode<Label>("ActionState");
            _actor.AddChild(stateDisplay);
            Vector2 frameSize = _actor.BodySprite.GetFrameSize();
            stateDisplay.RectPosition = new Vector2(_actor.BodySprite.Position.x, frameSize.y / 2 * -1 - 10 + _actor.BodySprite.Position.y);
        }
    }
}