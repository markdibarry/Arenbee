using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Extensions;
using GameCore.GUI.GameDialog;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DialogOptionSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    public Choice[] DialogChoices { get; set; } = Array.Empty<Choice>();
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    private OptionContainer _options = null!;

    public override void SetupData(object? data)
    {
        if (data is not IEnumerable<Choice> choices)
            return;
        DialogChoices = choices.ToArray();
    }

    protected override void OnItemSelected()
    {
        if (CurrentContainer?.FocusedItem == null || !CurrentContainer.FocusedItem.TryGetData("index", out int selectedIndex))
            return;
        var data = new List<Choice>(1) { DialogChoices[selectedIndex] };
        _ = CloseSubMenuAsync(data: data);
    }

    protected override void SetupOptions()
    {
        if (DialogChoices.Length == 0)
            return;
        List<TextOption> options = new();
        for (int i = 0; i < DialogChoices.Length; i++)
        {
            if (DialogChoices[i].Disabled)
                continue;
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.LabelText = DialogChoices[i].Text;
            textOption.OptionData["index"] = i;
            options.Add(textOption);
        }
        _options.ReplaceChildren(options);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _options = OptionContainers.First(x => x.Name == "OptionContainer");
    }
}
