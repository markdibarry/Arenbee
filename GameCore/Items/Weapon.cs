﻿using Godot;
using GameCore.Utility;
using GameCore.Actors;

namespace GameCore.Items;

public abstract partial class Weapon : Node2D
{
    public AnimationPlayer AnimationPlayer { get; set; }
    public ItemBase Item
    {
        get => Locator.ItemDB.GetItem(ItemId);
        private set => ItemId = value?.Id;
    }
    public string ItemId { get; private set; }
    public Sprite2D Sprite { get; set; }
    public string WeaponTypeName { get; set; }
    protected Actor Holder { get; set; }

    public override void _Ready()
    {
        SetNodeReferences();
        SetHitBoxes();
        GetActionStateMachine();
    }

    public void Init(Actor holder)
    {
        Holder = holder;
    }

    public abstract ActionStateMachineBase GetActionStateMachine();

    public void PlaySoundFX(string soundPath)
    {
        Holder.PlaySoundFX(soundPath);
    }

    public void PlaySoundFX(AudioStream sound)
    {
        Holder.PlaySoundFX(sound);
    }

    protected void SetItemId(string itemId)
    {
        Item = Locator.ItemDB.GetItem(itemId);
    }

    protected virtual void SetHitBoxes() { }

    protected virtual void SetNodeReferences()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = GetNode<Sprite2D>("Sprite");
    }
}