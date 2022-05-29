﻿using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI
{
    public partial class HUD : CanvasLayer
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        private HeartDisplay _heartDisplay;
        private MessageBoxList _messageBoxList;
        private IStatusEffectDB _statusEffectDB;

        public override void _Ready()
        {
            _messageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
            _heartDisplay = GetNode<HeartDisplay>("PlayerStatsDisplay/MarginWrapper/VBoxContainer/HeartDisplay");
            _statusEffectDB = Locator.GetStatusEffectDB();
        }

        public void OnActorAdded(Actor actor)
        {
            if (actor.ActorType == ActorType.Player)
                UpdatePlayerStatsDisplay(actor);
        }

        public void OnActorDamaged(Actor actor, DamageData data)
        {
            switch (data.ActionType)
            {
                case ActionType.Status:
                    DisplayStatusMessage(data);
                    break;
                case ActionType.Magic:
                case ActionType.Melee:
                    DisplayMeleeMessage(data);
                    break;
            }
        }

        public void OnActorDefeated(Actor actor)
        {
            string defeatedMessage = $"{actor.Name} was defeated!";
            AddMessage(defeatedMessage);
        }

        public void OnPlayerModChanged(Actor actor, ModChangeData data)
        {
            if (data.Modifier.StatType == StatType.StatusEffect)
            {
                string message;
                if (data.Change == ModChange.Add)
                    message = $"{data.Actor.Name} was {_statusEffectDB.GetEffectData(data.Modifier.SubType).PastTenseName}!";
                else
                    message = $"{data.Actor.Name} recovered from {_statusEffectDB.GetEffectData(data.Modifier.SubType).Name}!";

                AddMessage(message);
            }
        }

        public void OnPlayerStatsChanged(Actor actor)
        {
            UpdatePlayerStatsDisplay(actor);
        }

        private void AddMessage(string message)
        {
            if (ProcessMode == ProcessModeEnum.Disabled)
                return;
            _messageBoxList.AddMessageToTop(message);
        }

        private void DisplayMeleeMessage(DamageData data)
        {
            if (data.ElementMultiplier != ElementDef.None)
            {
                string effectiveness = GetEffectivenessMessage(data.ElementMultiplier);
                string effectiveMessage = $"{data.RecieverName} {effectiveness} {data.ElementDamage}!";
                AddMessage(effectiveMessage);
            }
            string action = data.TotalDamage < 0 ? "healed" : "hurt";
            string actionMessage = $"{data.SourceName} {action} {data.RecieverName} for {Math.Abs(data.TotalDamage)} HP!";
            AddMessage(actionMessage);
        }

        private void DisplayStatusMessage(DamageData data)
        {
            string actionMessage;
            if (data.TotalDamage > 0)
                actionMessage = $"{data.RecieverName} took {data.TotalDamage} {data.SourceName.ToLower()} damage!";
            else
                actionMessage = $"{data.RecieverName} was healed for {Math.Abs(data.TotalDamage)} HP!";
            AddMessage(actionMessage);
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
            if (ProcessMode == ProcessModeEnum.Disabled)
                return;
            int hp = actor.Stats.GetHP();
            int maxHP = actor.Stats.GetMaxHP();
            _heartDisplay.UpdateMaxHearts(maxHP);
            _heartDisplay.UpdateCurrentHearts(hp);
        }
    }
}