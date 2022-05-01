using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Godot;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Utility;

namespace Arenbee.Assets.GUI
{
    public partial class HUD : CanvasLayer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private MessageBoxList _messageBoxList;
        private Label _fpsDisplay;
        private HeartDisplay _heartDisplay;
        private IStatusEffectDB _statusEffectDB;

        public override void _Ready()
        {
            _messageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
            _fpsDisplay = GetNode<Label>("FPSDisplay");
            _heartDisplay = GetNode<HeartDisplay>("PlayerStatsDisplay/MarginWrapper/VBoxContainer/HeartDisplay");
            _statusEffectDB = Locator.GetStatusEffectDB();
        }

        public override void _Process(float delta)
        {
            _fpsDisplay.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
        }

        public void SubscribeAreaSceneEvents(AreaScene areaScene)
        {
            foreach (Actor actor in areaScene.GetAllActors())
                SubscribeActorEvents(actor);
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
                actor.StatsChanged += OnPlayerStatsChanged;
                actor.ModChanged += OnPlayerModChanged;
            }
        }

        private void UnsubscribeActorEvents(Actor actor)
        {
            actor.DamageRecieved -= OnDamageRecieved;
            actor.ActorDefeated -= OnActorDefeated;
            actor.ActorRemoved -= OnActorRemoved;
            if (actor.ActorType == ActorType.Player)
            {
                actor.ModChanged -= OnPlayerModChanged;
                actor.StatsChanged -= OnPlayerStatsChanged;
            }
        }

        private void OnPlayerStatsChanged(Actor actor)
        {
            if (ProcessMode == ProcessModeEnum.Disabled) return;
            UpdatePlayerStatsDisplay(actor);
        }

        private void OnDamageRecieved(DamageData data)
        {
            if (ProcessMode == ProcessModeEnum.Disabled) return;
            switch (data.ActionType)
            {
                case ActionType.Status:
                    DisplayStatusMessage(data);
                    break;
                case ActionType.Melee:
                    DisplayMeleeMessage(data);
                    break;
            }
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

        private void OnPlayerModChanged(ModChangeData data)
        {
            if (ProcessMode == ProcessModeEnum.Disabled) return;
            if (data.Modifier.StatType == StatType.StatusEffect)
            {
                string message;
                if (data.Change == ModChange.Add)
                    message = $"{data.Actor.Name} was {_statusEffectDB.GetEffectData(data.Modifier.SubType).PastTenseName}!";
                else
                    message = $"{data.Actor.Name} recovered from {_statusEffectDB.GetEffectData(data.Modifier.SubType).Name}!";

                _messageBoxList.AddMessageToTop(message);
            }
        }

        private void DisplayMeleeMessage(DamageData data)
        {
            if (data.ElementMultiplier != ElementDef.None)
            {
                string effectiveness = GetEffectivenessMessage(data.ElementMultiplier);
                string effectiveMessage = $"{data.RecieverName} {effectiveness} {data.ElementDamage}!";
                _messageBoxList.AddMessageToTop(effectiveMessage);
            }
            string action = data.TotalDamage < 0 ? "healed" : "hurt";
            string actionMessage = $"{data.SourceName} {action} {data.RecieverName} for {Math.Abs(data.TotalDamage)} HP!";
            _messageBoxList.AddMessageToTop(actionMessage);
        }

        private void DisplayStatusMessage(DamageData data)
        {
            string actionMessage;
            if (data.TotalDamage > 0)
                actionMessage = $"{data.RecieverName} took {data.TotalDamage} {data.SourceName.ToLower()} damage!";
            else
                actionMessage = $"{data.RecieverName} was healed for {Math.Abs(data.TotalDamage)} HP!";
            _messageBoxList.AddMessageToTop(actionMessage);
        }

        private string GetEffectivenessMessage(int elementMultiplier)
        {
            if (elementMultiplier > ElementDef.None)
                return "is weak to";
            else if (elementMultiplier == ElementDef.Resist)
                return "resists";
            else if (elementMultiplier == ElementDef.Nullify)
                return "nullifies";
            else
                return "absorbs";
        }

        private void UpdatePlayerStatsDisplay(Actor actor)
        {
            int hp = actor.Stats.GetHP();
            int maxHP = actor.Stats.GetMaxHP();
            _heartDisplay.UpdateMaxHearts(maxHP);
            _heartDisplay.UpdateCurrentHearts(hp);
        }
    }
}
