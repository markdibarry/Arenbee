using System.Collections.Generic;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DialogOptionSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    public Choice[] DialogChoices { get; set; }
    private PackedScene _textOptionScene;
    private OptionContainer _options;

    public override void SetupData(object data)
    {
        if (data is not DialogOptionDataModel dataModel)
            return;
        DialogChoices = dataModel.DialogChoices;
    }

    protected override void OnItemSelected()
    {
        int selectedIndex = CurrentContainer.CurrentItem.GetData<int>("index");
        var next = DialogChoices[selectedIndex].Next;
        DialogOptionSelectionDataModel data = new() { Next = next };
        _ = CloseSubMenuAsync(data: data);
    }

    protected override void SetupOptions()
    {
        if (DialogChoices == null || DialogChoices.Length == 0)
            return;
        _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        for (int i = 0; i < DialogChoices.Length; i++)
        {
            if (!DialogChoices[i].Selectable)
                continue;
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData["index"] = i;
            options.Add(textOption);
        }
        _options.ReplaceChildren(options);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _options = OptionContainers.Find(x => x.Name == "OptionContainer");
        //_options.FitContainer = true;
    }
}
