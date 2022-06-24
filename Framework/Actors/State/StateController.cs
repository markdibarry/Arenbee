using System;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class StateController
    {
        public StateController(
            Actor actor,
            MoveStateMachineBase moveStateMachineBase,
            AirStateMachineBase airStateMachineBase,
            HealthStateMachineBase healthStateMachineBase,
            ActionStateMachineBase actionStateMachineBase)
        {
            _actor = actor;
            HealthStateMachine = healthStateMachineBase;
            AirStateMachine = airStateMachineBase;
            MoveStateMachine = moveStateMachineBase;
            ActionStateMachine = actionStateMachineBase;
        }

        private readonly Actor _actor;
        private Label _airStateDisplay;
        private Label _moveStateDisplay;
        private Label _actionStateDisplay;
        private Label _healthStateDisplay;
        public AirStateMachineBase AirStateMachine { get; }
        public MoveStateMachineBase MoveStateMachine { get; }
        public ActionStateMachineBase ActionStateMachine { get; private set; }
        public HealthStateMachineBase HealthStateMachine { get; }
        public AnimationPlayer ActorAnimationPlayer => _actor.AnimationPlayer;
        public Weapon CurrentWeapon => _actor.WeaponSlot.CurrentWeapon;
        public AnimationPlayer WeaponAnimationPlayer => CurrentWeapon?.AnimationPlayer;

        public void CreateStateDisplay()
        {
            //Debug Only
            var stateDisplay = GDEx.Instantiate<Control>(StateDisplay.GetScenePath());
            _airStateDisplay = stateDisplay.GetNode<Label>("AirState");
            _moveStateDisplay = stateDisplay.GetNode<Label>("MoveState");
            _actionStateDisplay = stateDisplay.GetNode<Label>("ActionState");
            _healthStateDisplay = stateDisplay.GetNode<Label>("HealthState");
            _moveStateDisplay.Text = string.Empty;
            _airStateDisplay.Text = string.Empty;
            _actionStateDisplay.Text = string.Empty;
            _healthStateDisplay.Text = string.Empty;
            _actor.AddChild(stateDisplay);
            Vector2 frameSize = _actor.BodySprite.GetFrameSize();
            stateDisplay.Position = new Vector2(_actor.BodySprite.Position.x, (frameSize.y / 2 * -1) - 10 + _actor.BodySprite.Position.y);
        }

        public void SwitchActionStateMachine(ActionStateMachineBase actionStateMachineBase)
        {
            ActionStateMachine?.ExitState();
            ActionStateMachine = actionStateMachineBase;
            ActionStateMachine.Init();
        }

        public void Init()
        {
            HealthStateMachine.Init();
            AirStateMachine.Init();
            MoveStateMachine.Init();
            ActionStateMachine.Init();
            CreateStateDisplay();
        }

        public bool IsBlocked(BlockedState stateType)
        {
            return HealthStateMachine.State.BlockedStates.HasFlag(stateType) ||
                MoveStateMachine.State.BlockedStates.HasFlag(stateType) ||
                ActionStateMachine.State.BlockedStates.HasFlag(stateType) ||
                AirStateMachine.State.BlockedStates.HasFlag(stateType);
        }

        public bool PlayHealthAnimation(string animationName)
        {
            if (!ActorAnimationPlayer.HasAnimation(animationName))
                return false;
            if (CurrentWeapon != null && CurrentWeapon.AnimationPlayer.CurrentAnimation != "RESET")
                CurrentWeapon.AnimationPlayer.Play("RESET");
            ActorAnimationPlayer.Play(animationName);
            return true;
        }

        public bool PlayActionAnimation(string animationName)
        {
            if (HealthStateMachine.State.AnimationName != null)
                return false;
            string playerAnimPath = animationName;
            if (CurrentWeapon == null)
            {
                if (!ActorAnimationPlayer.HasAnimation(playerAnimPath))
                    return false;
            }
            else
            {
                playerAnimPath = $"{CurrentWeapon.WeaponTypeName}/{animationName}";
                if (!ActorAnimationPlayer.HasAnimation(playerAnimPath)
                    || !CurrentWeapon.AnimationPlayer.HasAnimation(animationName))
                    return false;
                CurrentWeapon.AnimationPlayer.Play(animationName);
            }
            ActorAnimationPlayer.Play(playerAnimPath);
            return true;
        }

        public bool PlayAirAnimation(string animationName)
        {
            if (HealthStateMachine.State.AnimationName != null
                || ActionStateMachine.State.AnimationName != null)
                return false;
            if (!ActorAnimationPlayer.HasAnimation(animationName))
                return false;
            if (CurrentWeapon != null && CurrentWeapon.AnimationPlayer.CurrentAnimation != "RESET")
                CurrentWeapon.AnimationPlayer.Play("RESET");
            ActorAnimationPlayer.Play(animationName);
            return true;
        }

        public bool PlayMoveAnimation(string animationName)
        {
            if (HealthStateMachine.State.AnimationName != null
                || ActionStateMachine.State.AnimationName != null
                || AirStateMachine.State.AnimationName != null)
                return false;
            if (!ActorAnimationPlayer.HasAnimation(animationName))
                return false;
            if (CurrentWeapon != null && CurrentWeapon.AnimationPlayer.CurrentAnimation != "RESET")
                CurrentWeapon.AnimationPlayer.Play("RESET");
            ActorAnimationPlayer.Play(animationName);
            return true;
        }

        // public void PlaySubWeaponAttack(string animationName)
        // {
        // }

        public bool PlayFallbackAnimation()
        {
            if (HealthStateMachine.State.AnimationName != null)
                return PlayHealthAnimation(HealthStateMachine.State.AnimationName);
            if (ActionStateMachine.State.AnimationName != null)
                return PlayActionAnimation(ActionStateMachine.State.AnimationName);
            else if (AirStateMachine.State.AnimationName != null)
                return PlayAirAnimation(AirStateMachine.State.AnimationName);
            else if (MoveStateMachine.State.AnimationName != null)
                return PlayMoveAnimation(MoveStateMachine.State.AnimationName);
            return false;
        }

        public void UpdateStates(float delta)
        {
            MoveStateMachine.Update(delta);
            AirStateMachine.Update(delta);
            ActionStateMachine.Update(delta);
            HealthStateMachine.Update(delta);
            _moveStateDisplay.Text = MoveStateMachine.State.GetType().Name;
            _airStateDisplay.Text = AirStateMachine.State.GetType().Name;
            _actionStateDisplay.Text = ActionStateMachine.State.GetType().Name;
            _healthStateDisplay.Text = HealthStateMachine.State.GetType().Name;
        }
    }
}