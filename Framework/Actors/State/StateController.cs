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
        public IState UnarmedInitialState { get; private set; }
        public bool AnimationOverride { get; set; }
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
            UnarmedInitialState = actionState;
            if (CurrentWeapon == null)
                ActionStateMachine.Init(actionState);
        }

        public void ResetMachines()
        {
            BaseStateMachine.TransitionTo(BaseStateMachine.InitialState);
            JumpStateMachine.TransitionTo(JumpStateMachine.InitialState);
            ActionStateMachine.TransitionTo(ActionStateMachine.InitialState);
        }

        public bool PlayAnimation(IStateMachine stateMachine, IState state, string animationName, bool force = false)
        {
            if (force)
            {
                if (PlayIfAvailable(animationName))
                {
                    AnimationOverride = true;
                    return true;
                }

                return false;
            }

            if (AnimationOverride) return false;

            return stateMachine.StateMachineType switch
            {
                StateMachineType.Action => PlayActionAnimation(animationName),
                StateMachineType.Jump => PlayJumpAnimation(animationName),
                StateMachineType.Base => PlayBaseAnimation(animationName),
                _ => false,
            };
        }

        public bool PlayActionAnimation(string animationName)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.UpdateHitBoxAction();
                ActorAnimationPlayer.Stop();
                CurrentWeapon.AnimationPlayer.Stop();
                if (ActorAnimationPlayer.HasAnimation(CurrentWeapon.WeaponTypeName + animationName)
                    && CurrentWeapon.AnimationPlayer.HasAnimation(animationName))
                {
                    PlayIfAvailable(CurrentWeapon.WeaponTypeName + animationName);
                    CurrentWeapon.AnimationPlayer.Play(animationName);
                    return true;
                }

                return false;
            }

            return PlayIfAvailable(animationName);
        }

        public bool PlayJumpAnimation(string animationName)
        {
            if (string.IsNullOrEmpty(ActionStateMachine.State.AnimationName))
                return PlayIfAvailable(animationName);
            return false;
        }

        public bool PlayBaseAnimation(string animationName)
        {
            if (string.IsNullOrEmpty(ActionStateMachine.State.AnimationName)
                && string.IsNullOrEmpty(JumpStateMachine.State.AnimationName))
            {
                return PlayIfAvailable(animationName);
            }
            return false;
        }

        // public void PlaySubWeaponAttack(string animationName)
        // {

        // }

        public bool PlayIfAvailable(string animationName)
        {
            if (!ActorAnimationPlayer.HasAnimation(animationName))
                return false;
            ActorAnimationPlayer.Play(animationName);
            return true;
        }

        public bool PlayFallbackAnimation()
        {
            string animation = null;
            if (!string.IsNullOrEmpty(ActionStateMachine.State.AnimationName))
                animation = ActionStateMachine.State.AnimationName;
            else if (!string.IsNullOrEmpty(JumpStateMachine.State.AnimationName))
                animation = JumpStateMachine.State.AnimationName;
            else if (!string.IsNullOrEmpty(BaseStateMachine.State.AnimationName))
                animation = BaseStateMachine.State.AnimationName;

            if (animation != null)
                return PlayIfAvailable(animation);
            return false;
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
            var stateDisplay = GDEx.Instantiate<Control>(PathConstants.StateDisplay);
            _jumpStateDisplay = stateDisplay.GetNode<Label>("JumpState");
            _moveStateDisplay = stateDisplay.GetNode<Label>("MoveState");
            _actionStateDisplay = stateDisplay.GetNode<Label>("ActionState");
            _moveStateDisplay.Text = string.Empty;
            _jumpStateDisplay.Text = string.Empty;
            _actionStateDisplay.Text = string.Empty;
            _actor.AddChild(stateDisplay);
            Vector2 frameSize = _actor.BodySprite.GetFrameSize();
            stateDisplay.RectPosition = new Vector2(_actor.BodySprite.Position.x, (frameSize.y / 2 * -1) - 10 + _actor.BodySprite.Position.y);
        }
    }
}