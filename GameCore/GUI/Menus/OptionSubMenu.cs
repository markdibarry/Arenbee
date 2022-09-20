using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    private PackedScene _cursorScene;
    private Cursor _cursor;
    public OptionContainer CurrentContainer { get; private set; }
    public List<OptionContainer> OptionContainers { get; private set; }
    protected string SelectedSoundPath { get; set; } = "menu_select1.wav";
    protected string FocusedSoundPath { get; set; } = "menu_bip1.wav";

    public override async void ResumeSubMenu()
    {
        await ToSignal(GetTree(), Signals.ProcessFrameSignal);
        FocusContainer(CurrentContainer);
        base.ResumeSubMenu();
    }

    protected sealed override void PreWaitFrameSetup()
    {
        if (!this.IsToolDebugMode())
            SetupOptions();
        base.PreWaitFrameSetup();
    }

    protected sealed override async Task PostWaitFrameSetup()
    {
        Modulate = TempColor;
        foreach (var optionContainer in OptionContainers)
        {
            if (optionContainer.FitContainer)
                optionContainer.FitToContent();
        }
        await TransitionOpenAsync();
        OptionContainers.ForEach(x => SubscribeToEvents(x));
        FocusContainer(OptionContainers.FirstOrDefault());
    }

    protected void FocusContainerClosestItem(OptionContainer optionContainer)
    {
        if (optionContainer == null)
            return;
        int index = CurrentContainer.CurrentItem.GetClosestIndex(optionContainer.OptionItems.AsEnumerable());
        FocusContainer(optionContainer, index);
    }

    protected void FocusContainer(OptionContainer optionContainer)
    {
        if (optionContainer == null)
            return;
        FocusContainer(optionContainer, optionContainer.CurrentIndex);
    }

    protected void FocusContainer(OptionContainer optionContainer, int index)
    {
        if (optionContainer == null || optionContainer.OptionItems.Count == 0)
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
        OptionContainers = Foreground.GetChildren<OptionContainer>().ToList();
        _cursorScene = GD.Load<PackedScene>(HandCursor.GetScenePath());
        AddCursor();
    }

    protected void AddCursor()
    {
        _cursor = Foreground.GetChildren<Cursor>().FirstOrDefault();
        if (_cursor == null)
        {
            _cursor = _cursorScene.Instantiate<Cursor>();
            Foreground.AddChild(_cursor);
        }
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
        float cursorX = optionItem.GlobalPosition.x - 4;
        float cursorY = (float)(optionItem.GlobalPosition.y + Math.Round(optionItem.Size.y * 0.5));
        _cursor.GlobalPosition = new Vector2(cursorX, cursorY);
    }

    private void OnContainerChanged(OptionContainer optionContainer)
    {
        if (optionContainer == CurrentContainer)
            MoveCursorToItem(optionContainer.CurrentItem);
    }

    private void OnItemFocusedBase()
    {
        _cursor.Visible = !CurrentContainer.AllSelected;
        if (CurrentContainer.LastIndex != CurrentContainer.CurrentIndex)
            Locator.Audio.PlaySoundFX(FocusedSoundPath);
        MoveCursorToItem(CurrentContainer.CurrentItem);
        OnItemFocused();
    }

    private void OnItemSelectedBase()
    {
        if (CurrentContainer.CurrentItem.Disabled)
        {
            Locator.Audio.PlaySoundFX(FocusedSoundPath);
            return;
        }

        Locator.Audio.PlaySoundFX(SelectedSoundPath);
        OnItemSelected();
    }
}
