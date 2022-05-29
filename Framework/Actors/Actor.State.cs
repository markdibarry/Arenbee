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
        public StateController StateController { get; protected set; }
        public IFrameController IFrameController { get; }
        public delegate void ActorHandler(Actor actor);
        public delegate void DamageReceivedHandler(Actor actor, DamageData damageRecievedData);
        public event ActorHandler Defeated;
        public event DamageReceivedHandler DamageRecieved;

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
                BodyShader.Dispose();
        }

        public void PlaySoundFX(string soundPath)
        {
            Locator.GetAudio().PlaySoundFX(this, soundPath);
        }

        public void PlaySoundFX(AudioStream sound)
        {
            Locator.GetAudio().PlaySoundFX(this, sound);
        }

        public abstract ActionStateMachineBase GetActionStateMachine();

        private void InitState()
        {
            StateController.Init();
            IFrameController.Init();
        }

        private void OnDamageRecieved(DamageData damageData)
        {
            damageData.RecieverName = Name;
            StateController.HealthStateMachine.State.HandleDamage(damageData);
            DamageRecieved?.Invoke(this, damageData);
        }

        private void OnHPDepleted()
        {
            StateController.HealthStateMachine.State.HandleHPDepleted();
            Defeated?.Invoke(this);
        }
    }
}
