using System;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class StateController
    {
        public StateController(Actor actor)
        {
            _actor = actor;
            MoveStateMachine = new ActorStateMachine(_actor, this);
            JumpStateMachine = new ActorStateMachine(_actor, this);
            ActionStateMachine = new ActorStateMachine(_actor, this);
            HealthStateMachine = new ActorStateMachine(_actor, this);
        }

        private readonly Actor _actor;
        private Label _jumpStateDisplay;
        private Label _moveStateDisplay;
        private Label _actionStateDisplay;
        public ActorStateMachine JumpStateMachine { get; }
        public ActorStateMachine MoveStateMachine { get; }
        public ActorStateMachine ActionStateMachine { get; }
        public ActorStateMachine HealthStateMachine { get; }
        public AnimationPlayer ActorAnimationPlayer => _actor.AnimationPlayer;
        public Weapon CurrentWeapon => _actor.WeaponSlot.CurrentWeapon;
        public AnimationPlayer WeaponAnimationPlayer => CurrentWeapon?.AnimationPlayer;

        public void CreateStateDisplay()
        {
            //Debug Only
            var stateDisplay = GDEx.Instantiate<Control>(StateDisplay.GetScenePath());
            _jumpStateDisplay = stateDisplay.GetNode<Label>("JumpState");
            _moveStateDisplay = stateDisplay.GetNode<Label>("MoveState");
            _actionStateDisplay = stateDisplay.GetNode<Label>("ActionState");
            _moveStateDisplay.Text = string.Empty;
            _jumpStateDisplay.Text = string.Empty;
            _actionStateDisplay.Text = string.Empty;
            _actor.AddChild(stateDisplay);
            Vector2 frameSize = _actor.BodySprite.GetFrameSize();
            stateDisplay.Position = new Vector2(_actor.BodySprite.Position.x, (frameSize.y / 2 * -1) - 10 + _actor.BodySprite.Position.y);
        }

        public void Init<TMoveState, TJumpState, THealthState>()
            where TMoveState : MoveState, new()
            where TJumpState : JumpState, new()
            where THealthState : HealthState, new()
        {
            HealthStateMachine.TransitionTo<THealthState>();
            MoveStateMachine.TransitionTo<TMoveState>();
            JumpStateMachine.TransitionTo<TJumpState>();
            if (CurrentWeapon == null)
                _actor.InitActionState();
        }

        public bool IsBlocked(ActorStateType stateType)
        {
            if (Array.IndexOf(HealthStateMachine.State.BlockedStates, stateType) != -1)
                return true;
            if (Array.IndexOf(MoveStateMachine.State.BlockedStates, stateType) != -1)
                return true;
            else if (Array.IndexOf(ActionStateMachine.State.BlockedStates, stateType) != -1)
                return true;
            else if (Array.IndexOf(JumpStateMachine.State.BlockedStates, stateType) != -1)
                return true;
            return false;
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
            }
            CurrentWeapon.AnimationPlayer.Play(animationName);
            ActorAnimationPlayer.Play(playerAnimPath);
            return true;
        }

        public bool PlayJumpAnimation(string animationName)
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
                || JumpStateMachine.State.AnimationName != null)
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
            else if (JumpStateMachine.State.AnimationName != null)
                return PlayJumpAnimation(JumpStateMachine.State.AnimationName);
            else if (MoveStateMachine.State.AnimationName != null)
                return PlayMoveAnimation(MoveStateMachine.State.AnimationName);
            return false;
        }

        public void UpdateStates(float delta)
        {
            MoveStateMachine.Update(delta);
            JumpStateMachine.Update(delta);
            ActionStateMachine.Update(delta);
            HealthStateMachine.Update(delta);
            _moveStateDisplay.Text = MoveStateMachine.State.GetType().Name;
            _jumpStateDisplay.Text = JumpStateMachine.State.GetType().Name;
            _actionStateDisplay.Text = ActionStateMachine.State.GetType().Name;
        }
    }

    public enum StateMachineType
    {
        Move,
        Jump,
        Action
    }
}