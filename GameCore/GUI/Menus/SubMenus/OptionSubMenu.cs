using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class OptionSubMenu : SubMenu
{
    public OptionSubMenu()
    {
        OptionContainers = new List<OptionContainer>();
        _currentDirection = Direction.None;
    }

    private PackedScene _cursorScene = GD.Load<PackedScene>(HandCursor.GetScenePath());
    private OptionCursor _cursor = null!;
    [Export] public Godot.Collections.Array<NodePath> NodePaths { get; set; } = new();
    public OptionContainer CurrentContainer { get; private set; } = null!;
    public List<OptionContainer> OptionContainers { get; private set; }
    protected string SelectedSoundPath { get; set; } = "menu_select1.wav";
    protected string FocusedSoundPath { get; set; } = "menu_bip1.wav";

    public override async void ResumeSubMenu()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
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
        OptionContainers.ForEach(x => SubscribeToEvents(x));
        FocusContainer(OptionContainers.First());
    }

    protected void FocusContainerClosestItem(OptionContainer optionContainer)
    {
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

    private void MoveCursorToItem(OptionItem optionItem)
    {
        if (CurrentContainer.AllSelected)
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
        _cursor.Visible = !CurrentContainer.AllSelected;
        if (CurrentContainer.PreviousIndex != CurrentContainer.FocusedIndex)
            Locator.Audio.PlaySoundFX(FocusedSoundPath);
        MoveCursorToItem(CurrentContainer.FocusedItem);
        OnItemFocused();
    }

    private void OnItemSelectedBase()
    {
        if (CurrentContainer.FocusedItem.Disabled)
        {
            Locator.Audio.PlaySoundFX(FocusedSoundPath);
            return;
        }

        Locator.Audio.PlaySoundFX(SelectedSoundPath);
        OnItemSelected();
    }
}
