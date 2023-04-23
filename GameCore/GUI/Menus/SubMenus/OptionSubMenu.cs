﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Enums;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class OptionSubMenu : SubMenu
{
    private readonly PackedScene _cursorScene = GD.Load<PackedScene>(HandCursor.GetScenePath());
    private OptionCursor _cursor = null!;
    [Export] public Godot.Collections.Array<NodePath> NodePaths { get; set; } = new();
    public OptionContainer? CurrentContainer { get; private set; }
    public List<OptionContainer> OptionContainers { get; } = new();
    protected string SelectedSoundPath { get; set; } = "menu_select1.wav";
    protected string FocusedSoundPath { get; set; } = "menu_bip1.wav";

    public override async void ResumeSubMenu()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        if (CurrentContainer == null)
            throw new Exception("Current container is null!");
        FocusContainer(CurrentContainer);
        base.ResumeSubMenu();
    }

    protected sealed override void PreWaitFrameSetup()
    {
        SetupOptions();
        base.PreWaitFrameSetup();
    }

    protected sealed override void PostWaitFrameSetup()
    {
        if (OptionContainers.Count == 0)
            return;
        OptionContainers.ForEach(SubscribeToEvents);
        FocusContainer(OptionContainers.First());
    }

    protected void FocusContainerClosestItem(OptionContainer optionContainer)
    {
        if (CurrentContainer?.FocusedItem == null)
            return;
        int index = optionContainer.OptionItems.GetClosestIndex(CurrentContainer.FocusedItem);
        FocusContainer(optionContainer, index);
    }

    protected void FocusContainer(OptionContainer optionContainer)
    {
        FocusContainer(optionContainer, optionContainer.FocusedIndex);
    }

    protected void FocusContainer(OptionContainer optionContainer, int index)
    {
        if (optionContainer.OptionItems.Count == 0)
            return;
        CurrentContainer?.LeaveContainerFocus();
        OnFocusContainer(optionContainer);
        CurrentContainer = optionContainer;
        optionContainer.FocusContainer(index);
    }

    protected virtual void OnFocusContainer(OptionContainer optionContainer) { }

    protected virtual void OnFocusOOB(OptionContainer container, Direction direction) { }

    protected virtual void OnItemFocused() { }

    protected virtual void OnItemSelected() { }

    /// <summary>
    /// Overrides the items that should display
    /// </summary>
    protected virtual void SetupOptions() { }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        foreach (var nodePath in NodePaths)
        {
            var container = GetNode<OptionContainer>(nodePath);
            if (container == null)
            {
                GD.PrintErr($"{nodePath} not found!");
                continue;
            }
            OptionContainers.Add(container);
        }
        AddCursor();
    }

    protected void AddCursor()
    {
        OptionCursor? cursor = Foreground.GetChildren<OptionCursor>().FirstOrDefault();
        if (cursor == null)
        {
            cursor = _cursorScene.Instantiate<OptionCursor>();
            Foreground.AddChild(cursor);
        }
        _cursor = cursor;
        _cursor.Visible = false;
    }

    protected void SubscribeToEvents(OptionContainer optionContainer)
    {
        optionContainer.ItemSelected += OnItemSelectedBase;
        optionContainer.ItemFocused += OnItemFocusedBase;
        optionContainer.FocusOOB += OnFocusOOB;
        optionContainer.ContainerUpdated += OnContainerChanged;
    }

    private void MoveCursorToItem(OptionItem? optionItem)
    {
        if (optionItem == null)
            return;
        _cursor.MoveToTarget(optionItem);
    }

    private void OnContainerChanged(OptionContainer optionContainer)
    {
        if (optionContainer == CurrentContainer)
            MoveCursorToItem(optionContainer.FocusedItem);
    }

    private void OnItemFocusedBase()
    {
        _cursor.Visible = CurrentContainer?.FocusedItem != null;
        if (CurrentContainer != null)
        {
            if (CurrentState != State.Opening)
                Audio.PlaySoundFX(FocusedSoundPath);
            MoveCursorToItem(CurrentContainer.FocusedItem);
        }
        OnItemFocused();
    }

    private void OnItemSelectedBase()
    {
        if (CurrentContainer == null || !CurrentContainer.AllSelected &&
            (CurrentContainer.FocusedItem == null || CurrentContainer.FocusedItem.Disabled))
        {
            Audio.PlaySoundFX(FocusedSoundPath);
            return;
        }

        Audio.PlaySoundFX(SelectedSoundPath);
        OnItemSelected();
    }
}
