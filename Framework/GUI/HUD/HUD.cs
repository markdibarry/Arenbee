using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class HUD : CanvasLayer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private PackedScene _timedMessageBoxScene;
        private MessageBoxList _messageBoxList;
        private Label _fpsDisplay;
        private Label _playerStatsDisplay;

        public override void _Ready()
        {
            _messageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
            _timedMessageBoxScene = GD.Load<PackedScene>(TimedMessageBox.GetScenePath());
            _fpsDisplay = GetNode<Label>("FPSDisplay");
            _playerStatsDisplay = GetNode<Label>("PlayerStatsDisplay/MarginWrapper/Panel/HP");
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
            actor.HitBoxActionRecieved += OnHitBoxActionRecieved;
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
            actor.HitBoxActionRecieved -= OnHitBoxActionRecieved;
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

        private void OnHitBoxActionRecieved(HitBoxActionRecievedData data)
        {
            if (data.ElementMultiplier != 1)
            {
                var effectiveMessage = _timedMessageBoxScene.Instantiate() as TimedMessageBox;
                string effectiveness = GetEffectivenessMessage(data.ElementMultiplier);
                effectiveMessage.MessageText = $"{data.RecieverName} {effectiveness} {data.Element}!";
                _messageBoxList.AddMessageToTop(effectiveMessage);
            }
            var actionMessage = _timedMessageBoxScene.Instantiate() as TimedMessageBox;
            string action = data.TotalDamage < 0 ? "healed" : "hurt";
            actionMessage.MessageText = $"{data.SourceName} {action} {data.RecieverName} for {Math.Abs(data.TotalDamage)} HP!";
            _messageBoxList.AddMessageToTop(actionMessage);
        }

        private void OnActorDefeated(Actor actor)
        {
            var defeatedMessage = _timedMessageBoxScene.Instantiate() as TimedMessageBox;
            defeatedMessage.MessageText = $"{actor.Name} was defeated!";
            _messageBoxList.AddMessageToTop(defeatedMessage);
        }

        private void OnActorRemoved(Actor actor)
        {
            UnsubscribeActorEvents(actor);
        }

        private string GetEffectivenessMessage(float elementMultiplier)
        {
            if (elementMultiplier > 1)
            {
                return "is weak to";
            }
            else if (0 < elementMultiplier && elementMultiplier < 1)
            {
                return "resists";
            }
            else if (elementMultiplier == 0)
            {
                return "nullifies";
            }
            else
            {
                return "absorbs";
            }
        }

        private void UpdatePlayerStatsDisplay(Actor actor)
        {
            _playerStatsDisplay.Text = $"{actor.Stats[StatType.HP].DisplayValue}/{actor.Stats[StatType.MaxHP].DisplayValue}";
        }
    }
}
