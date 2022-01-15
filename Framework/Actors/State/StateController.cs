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
            BaseStateMachine = new StateMachine(_actor, this);
            ActionStateMachine = new StateMachine(_actor, this);
            CreateStateDisplay();
        }

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
        private Label _moveStateDisplay;
        private Label _actionStateDisplay;
        public bool IsNonBaseActive { get; set; }
        /// <summary>
        /// If the Actor is able to switch states
        /// </summary>
        /// <value></value>
        public bool IsAvailable { get; set; }
        private string _lastBaseAnimation;

        public void Init(IState baseState, IState actionState)
        {
            BaseStateMachine.Init(baseState);
            if (CurrentWeapon == null)
                ActionStateMachine.Init(actionState);
        }

        public void TransitionToInit()
        {
            BaseStateMachine.TransitionTo(BaseStateMachine.InitialState);
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
            _lastBaseAnimation = animationName;
            if (ActionStateMachine.State.IsInitialState)
            {
                ActorAnimationPlayer.Play(animationName);
            }
        }

        public void PlayLastBaseAnimation()
        {
            PlayBase(_lastBaseAnimation);
        }

        public void UpdateStates()
        {
            BaseStateMachine.Update();
            ActionStateMachine.Update();
            _moveStateDisplay.Text = BaseStateMachine.State.GetType().Name;
            _actionStateDisplay.Text = ActionStateMachine.State.GetType().Name;
        }

        private void CreateStateDisplay()
        {
            //Debug Only
            var stateDisplay = GD.Load<PackedScene>(PathConstants.StateDisplay).Instantiate<Control>();
            _moveStateDisplay = stateDisplay.GetNode<Label>("MoveState");
            _actionStateDisplay = stateDisplay.GetNode<Label>("ActionState");
            _actor.AddChild(stateDisplay);
            Vector2 frameSize = _actor.BodySprite.GetFrameSize();
            stateDisplay.RectPosition = new Vector2(_actor.BodySprite.Position.x, frameSize.y / 2 * -1 - 10 + _actor.BodySprite.Position.y);
        }
    }
}