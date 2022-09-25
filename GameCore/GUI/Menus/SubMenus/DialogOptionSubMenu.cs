using System.Collections.Generic;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DialogOptionSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    public DialogChoice[] DialogChoices { get; set; }
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
        int next = CurrentContainer.CurrentItem.GetData<int>("next");
        DialogOptionSelectionDataModel data = new()
        {
            Next = next
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
