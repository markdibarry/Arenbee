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

    public override void ReceiveData(object data)
    {
        if (data is not DialogOptionDataModel dataModel)
            return;
        DialogChoices = dataModel.DialogChoices;
    }

    protected override void OnItemSelected()
    {
        string next = CurrentContainer.CurrentItem.GetData<string>("next");
        Line[] lines = CurrentContainer.CurrentItem.GetData<Line[]>("lines");
        DialogOptionSelectionDataModel data = new()
        {
            Next = next,
            Lines = lines
        };
        var closeRequest = new GUICloseRequest() { Data = data };
        RequestCloseSubMenu(closeRequest);
    }

    protected override void SetupOptions()
    {
        if (DialogChoices == null || DialogChoices.Length == 0)
            return;
        _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        foreach (var choice in DialogChoices)
        {
            if (choice.Condition != null && !choice.Condition.Evaluate())
                continue;
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData["next"] = choice.Next;
            textOption.OptionData["lines"] = choice.Lines;
            textOption.LabelText = choice.Text;
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
