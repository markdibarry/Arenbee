using Arenbee.Framework.Constants;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class StateController
    {
        public StateController(Actor actor)
        {
            _actor = actor;
            JumpStateMachine = new StateMachine(_actor, this);
            BaseStateMachine = new StateMachine(_actor, this);
            ActionStateMachine = new StateMachine(_actor, this);
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

        public void PlayWeaponAttack(string animationName)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.SetHitBoxAction();
                ActorAnimationPlayer.Stop();
                ActorAnimationPlayer.Play(CurrentWeapon.WeaponTypeName + animationName);
                CurrentWeapon.AnimationPlayer.Stop();
                CurrentWeapon.AnimationPlayer.Play(animationName);
            }
            else
            {
                ActorAnimationPlayer.Play(animationName);
            }
        }

        // public void PlaySubWeaponAttack(string animationName)
        // {

        // }

        public void PlayBase(string animationName)
        {
            if (ActionStateMachine.State.IsInitialState)
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
            if (animation != null) PlayBase(animation);
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