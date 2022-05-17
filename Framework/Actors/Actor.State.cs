using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        public int ContextAreasActive { get; set; }
        public Sprite2D BodySprite { get; private set; }
        public AnimationPlayer AnimationPlayer { get; private set; }
        public StateController StateController { get; private set; }
        public IFrameController IFrameController { get; }
        public delegate void ActorHandler(Actor actor);
        public delegate void DamageReceivedHandler(Actor actor, DamageData damageRecievedData);
        public event ActorHandler Defeated;
        public event DamageReceivedHandler DamageRecieved;

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
                IFrameController.HandleDispose();
        }

        public void PlaySoundFX(string soundPath)
        {
            Locator.GetAudio().PlaySoundFX(this, soundPath);
        }

        public void PlaySoundFX(AudioStream sound)
        {
            Locator.GetAudio().PlaySoundFX(this, sound);
        }

        public abstract void InitActionState();

        private void InitState()
        {
            StateController.CreateStateDisplay();
            IFrameController.Init();
        }

        protected virtual void HandleHPDepleted() { }

        private void OnDamageRecieved(DamageData damageData)
        {
            damageData.RecieverName = Name;
            ((HealthState)StateController.HealthStateMachine.State).HandleDamage(damageData);
            DamageRecieved?.Invoke(this, damageData);
        }

        private void OnHPDepleted()
        {
            HandleHPDepleted();
            Defeated?.Invoke(this);
        }
    }
}
