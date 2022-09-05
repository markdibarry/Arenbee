﻿using GameCore.Actors;

namespace Arenbee.Actors.Enemies;

public partial class Whisp : Actor
{
    public class AirStateMachine : AirStateMachineBase
    {
        public AirStateMachine(Actor actor)
            : base(actor)
        {
            AddState<Floating>();
            InitStates(this);
        }

        private class Floating : AirState
        {
            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
            }

            public override AirState Update(double delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override AirState CheckForTransitions()
            {
                return null;
            }
        }
    }
}
