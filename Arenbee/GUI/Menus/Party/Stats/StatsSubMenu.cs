using System.Collections.Generic;
using Arenbee.GUI.Menus.Common;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;
using Arenbee.Game;

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
        _partyOptions = OptionContainers.Find(x => x.Name == "PartyOptions");
        _statsDisplay = Foreground.GetNode<ActorStatsDisplay>("StatsDisplay");
        GameSession? gameSession = Locator.Session as GameSession;
        _playerParty = gameSession?.Party ?? new PlayerParty();
    }

    protected override void SetupOptions()
    {
        UpdatePartyMemberOptions();
    }

    protected override void OnItemFocused()
    {
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(AActor), out AActor? actor))
            return;
        _statsDisplay.UpdateStatsDisplay(null, actor.Stats);
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        var options = new List<TextOption>();
        foreach (var actor in _playerParty.Actors)
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData[nameof(AActor)] = actor;
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
