﻿using GameCore.Actors;

namespace Arenbee.Actors.Enemies.Default.State;

public class AirStateMachine : AirStateMachineBase
{
    public AirStateMachine(Actor actor)
        : base(actor)
    {
        AddState<Grounded>();
        AddState<Falling>();
        InitStates(this);
    }

    public class Grounded : AirState
    {
        public override void Enter()
        {
            Actor.VelocityY = (float)Actor.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override AirState Update(double delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override AirState CheckForTransitions()
        {
            if (!Actor.IsOnFloor())
                return GetState<Falling>();
            return null;
        }
    }

    public class Falling : AirState
    {
        public override void Enter() { }

        public override AirState Update(double delta)
        {
            Actor.ApplyFallGravity(delta);
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override AirState CheckForTransitions()
        {
            if (Actor.IsOnFloor())
                return GetState<Grounded>();
            return null;
        }
    }
}
