using System.Collections.Generic;
using Arenbee.GUI.Menus.Common;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Party;

[Tool]
public partial class StatsSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private PlayerParty _playerParty;
    private ActorStatsDisplay _statsDisplay;
    private OptionContainer _partyOptions;
    private PackedScene _textOptionScene;

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        _partyOptions = OptionContainers.Find(x => x.Name == "PartyList");
        _statsDisplay = Foreground.GetNode<ActorStatsDisplay>("StatsDisplay");
        _playerParty = Locator.GetParty() ?? new PlayerParty();
    }

    protected override void ReplaceDefaultOptions()
    {
        UpdatePartyMemberOptions();
    }

    protected override void OnItemFocused()
    {
        base.OnItemFocused();
        var actor = CurrentContainer.CurrentItem.GetData<ActorBase>("actor");
        if (actor == null)
            return;
        _statsDisplay.UpdateStatsDisplay(actor);
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        var options = new List<TextOption>();
        foreach (var actor in _playerParty.Actors)
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData["actor"] = actor;
            textOption.LabelText = actor.Name;
            options.Add(textOption);
        }
        return options;
    }

    private void UpdatePartyMemberOptions()
    {
        List<TextOption> options = GetPartyMemberOptions();
        _partyOptions.ReplaceChildren(options);
    }
}
