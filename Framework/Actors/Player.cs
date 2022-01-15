using Arenbee.Assets.Players.MoveStates;
using Arenbee.Framework.Actors.Stats;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Player : Actor
    {
        public delegate void PlayerDefeatedHandler(string playerName);
        public event PlayerDefeatedHandler PlayerDefeated;

        public override void OnHitBoxActionRecieved(HitBoxActionRecievedData data)
        {
            base.OnHitBoxActionRecieved(data);

            if (data.TotalDamage > 0)
                StateController.BaseStateMachine.TransitionTo(new Stagger());
        }

        protected override void HandleHPDepleted()
        {
            PlayerDefeated?.Invoke(Name);
            StateController.BaseStateMachine.TransitionTo(new Dead());
        }
    }
}
