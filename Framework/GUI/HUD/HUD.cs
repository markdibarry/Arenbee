﻿using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class HUD : CanvasLayer
    {
        public static readonly string ScenePath = $"res://Framework/GUI/HUD/{nameof(HUD)}.tscn";
        public MessageBoxList MessageBoxList { get; set; }
        public PackedScene TimedMessageBox { get; set; }
        public Label FPSDisplay { get; set; }
        public Label PlayerStatsDisplay { get; set; }

        public override void _Ready()
        {
            base._Ready();
            MessageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
            TimedMessageBox = GD.Load<PackedScene>(GUI.TimedMessageBox.ScenePath);
            FPSDisplay = GetNode<Label>("FPSDisplay");
            PlayerStatsDisplay = GetNode<Label>("PlayerStatsDisplay/MarginWrapper/Panel/HP");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            FPSDisplay.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
        }

        public void SubscribeEvents(IEnumerable<Actor> actors)
        {
            foreach (Actor actor in actors)
            {
                SubscribeActorEvents(actor);
            }
        }

        public void SubscribeActorEvents(Actor actor)
        {
            actor.HitBoxActionRecieved += OnHitBoxActionRecieved;
            actor.ActorDefeated += OnActorDefeated;
            if (actor.ActorType == ActorType.Player)
            {
                UpdatePlayerStatsDisplay(actor);
                actor.StatsUpdated += OnPlayerStatsUpdated;
            }
        }

        public void UnsubscribeActorEvents(Actor actor)
        {
            actor.HitBoxActionRecieved -= OnHitBoxActionRecieved;
            actor.ActorDefeated -= OnActorDefeated;
            if (actor.ActorType == ActorType.Player)
            {
                actor.StatsUpdated -= OnPlayerStatsUpdated;
            }
        }

        private void OnPlayerStatsUpdated(Actor actor)
        {
            UpdatePlayerStatsDisplay(actor);
        }

        private void UpdatePlayerStatsDisplay(Actor actor)
        {
            PlayerStatsDisplay.Text = $"{actor.Stats[StatType.HP].DisplayValue}/{actor.Stats[StatType.MaxHP].DisplayValue}";
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

        private void OnActorDefeated(Actor actor)
        {
            var defeatedMessage = TimedMessageBox.Instantiate() as TimedMessageBox;
            defeatedMessage.MessageText = $"{actor.Name} was defeated!";
            MessageBoxList.AddMessageToTop(defeatedMessage);
            UnsubscribeActorEvents(actor);
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
