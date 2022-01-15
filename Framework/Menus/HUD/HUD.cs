using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Framework.Menus.HUD
{
    public partial class HUD : CanvasLayer
    {
        public MessageBoxList MessageBoxList { get; set; }
        public PackedScene TimedMessageBox { get; set; }
        public Label FPSDisplay { get; set; }
        public Label PlayerStatsDisplay { get; set; }

        public override void _Ready()
        {
            base._Ready();
            MessageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
            TimedMessageBox = GD.Load<PackedScene>(PathConstants.TimedMessageBox);
            FPSDisplay = GetNode<Label>("FPSDisplay");
            PlayerStatsDisplay = GetNode<Label>("PlayerStatsDisplay/MarginWrapper/Panel/HP");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            FPSDisplay.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
        }

        public void SubscribeToEvents(IEnumerable<Actor> actors)
        {
            foreach (Actor actor in actors)
            {
                actor.ActorStats.HitBoxActionRecieved += OnHitBoxActionRecieved;
                if (actor is Enemy enemy)
                {
                    enemy.EnemyDefeated += OnEnemyDefeated;
                }

                if (actor is Player player)
                {
                    player.PlayerDefeated += OnPlayerDefeated;
                    UpdatePlayerStatsDisplay(player.ActorStats);
                    player.StatsUpdated += OnPlayerStatsUpdated;
                }
            }
        }

        private void OnPlayerStatsUpdated(ActorStats actorStats)
        {
            UpdatePlayerStatsDisplay(actorStats);
        }

        private void UpdatePlayerStatsDisplay(ActorStats actorStats)
        {
            PlayerStatsDisplay.Text = $"{actorStats.Stats[StatType.HP].DisplayValue}/{actorStats.Stats[StatType.MaxHP].DisplayValue}";
        }

        private void OnHitBoxActionRecieved(HitBoxActionRecievedData data)
        {
            if (data.ElementMultiplier != 1)
            {
                var effectiveMessage = TimedMessageBox.Instantiate() as TimedMessageBox;
                string effectiveness = GetEffectivenessMessage(data.ElementMultiplier);
                effectiveMessage.MessageText = $"{data.RecieverName} {effectiveness} {data.Element}!";
                MessageBoxList.AddMessageToTop(effectiveMessage);
            }
            var actionMessage = TimedMessageBox.Instantiate() as TimedMessageBox;
            string action = data.TotalDamage < 0 ? "healed" : "hurt";
            actionMessage.MessageText = $"{data.SourceName} {action} {data.RecieverName} for {Math.Abs(data.TotalDamage)} HP!";
            MessageBoxList.AddMessageToTop(actionMessage);
        }

        private void OnEnemyDefeated(string enemyName)
        {
            var defeatedMessage = TimedMessageBox.Instantiate() as TimedMessageBox;
            defeatedMessage.MessageText = $"{enemyName} was defeated!";
            MessageBoxList.AddMessageToTop(defeatedMessage);
        }

        private void OnPlayerDefeated(string playerName)
        {
            var defeatedMessage = TimedMessageBox.Instantiate() as TimedMessageBox;
            defeatedMessage.MessageText = $"{playerName} was defeated!";
            MessageBoxList.AddMessageToTop(defeatedMessage);
        }

        private string GetEffectivenessMessage(float elementMultiplier)
        {
            string result;
            if (elementMultiplier > 1)
            {
                result = "is weak to";
            }
            else if (0 < elementMultiplier && elementMultiplier < 1)
            {
                result = "resists";
            }
            else if (elementMultiplier == 0)
            {
                result = "nullifies";
            }
            else
            {
                result = "absorbs";
            }
            return result;
        }
    }
}
