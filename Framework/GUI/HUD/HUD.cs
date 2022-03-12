using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class HUD : CanvasLayer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private MessageBoxList _messageBoxList;
        private Label _fpsDisplay;
        private Label _hpDisplay;

        public override void _Ready()
        {
            _messageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
            _fpsDisplay = GetNode<Label>("FPSDisplay");
            _hpDisplay = GetNode<Label>("PlayerStatsDisplay/MarginWrapper/VBoxContainer/HBoxContainer/HP");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            _fpsDisplay.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
        }

        public void SubscribeAreaSceneEvents(AreaScene areaScene)
        {
            var actors = areaScene.GetAllActors();
            foreach (Actor actor in actors)
            {
                SubscribeActorEvents(actor);
            }
            areaScene.ActorAdded += OnActorAddedToAreaScene;
        }

        private void OnActorAddedToAreaScene(Actor actor)
        {
            SubscribeActorEvents(actor);
        }

        private void SubscribeActorEvents(Actor actor)
        {
            actor.DamageRecieved += OnDamageRecieved;
            actor.ActorDefeated += OnActorDefeated;
            actor.ActorRemoved += OnActorRemoved;
            if (actor.ActorType == ActorType.Player)
            {
                UpdatePlayerStatsDisplay(actor);
                actor.StatsUpdated += OnPlayerStatsUpdated;
            }
        }

        private void UnsubscribeActorEvents(Actor actor)
        {
            actor.DamageRecieved -= OnDamageRecieved;
            actor.ActorDefeated -= OnActorDefeated;
            actor.ActorRemoved -= OnActorRemoved;
            if (actor.ActorType == ActorType.Player)
            {
                actor.StatsUpdated -= OnPlayerStatsUpdated;
            }
        }

        private void OnPlayerStatsUpdated(Actor actor)
        {
            UpdatePlayerStatsDisplay(actor);
        }

        private void OnDamageRecieved(DamageData data)
        {
            if (data.ElementMultiplier != ElementDefense.None)
            {
                string effectiveness = GetEffectivenessMessage(data.ElementMultiplier);
                string effectiveMessage = $"{data.RecieverName} {effectiveness} {data.Element}!";
                _messageBoxList.AddMessageToTop(effectiveMessage);
            }
            string action = data.TotalDamage < 0 ? "healed" : "hurt";
            string actionMessage = $"{data.SourceName} {action} {data.RecieverName} for {Math.Abs(data.TotalDamage)} HP!";
            _messageBoxList.AddMessageToTop(actionMessage);
        }

        private void OnActorDefeated(Actor actor)
        {
            string defeatedMessage = $"{actor.Name} was defeated!";
            _messageBoxList.AddMessageToTop(defeatedMessage);
        }

        private void OnActorRemoved(Actor actor)
        {
            UnsubscribeActorEvents(actor);
        }

        private string GetEffectivenessMessage(int elementMultiplier)
        {
            if (elementMultiplier > ElementDefense.None)
                return "is weak to";
            else if (elementMultiplier == ElementDefense.Resist)
                return "resists";
            else if (elementMultiplier == ElementDefense.Nullify)
                return "nullifies";
            else
                return "absorbs";
        }

        private void UpdatePlayerStatsDisplay(Actor actor)
        {
            int hp = actor.Stats.Attributes[AttributeType.HP].DisplayValue;
            int maxHP = actor.Stats.Attributes[AttributeType.MaxHP].DisplayValue;
            _hpDisplay.Text = $"{hp}/{maxHP}";
        }
    }
}
