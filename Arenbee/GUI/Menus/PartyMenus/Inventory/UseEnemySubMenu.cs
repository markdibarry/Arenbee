using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.ActionEffects;
using Arenbee.Actors;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.AreaScenes;
using GameCore;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class UseEnemySubMenu : OptionSubMenu
{
    public UseEnemySubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _inventory = gameSession?.MainParty?.Inventory ?? new Inventory();
        _gameSession = gameSession!;
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly IActionEffectDB _actionEffectDB = Locator.ActionEffectDB;
    private readonly AAreaScene? _areaScene = Locator.Session?.CurrentAreaScene;
    private readonly GameSession _gameSession;
    private readonly Inventory _inventory;
    private IActionEffect _actionEffect = null!;
    private List<AActorBody> _enemies = new();
    private ItemStack _itemStack = null!;
    private PackedScene _noDisplayOptionScene = GD.Load<PackedScene>(NoDisplayOption.GetScenePath());
    private OptionContainer _enemyContainer = null!;
    private AItem Item => _itemStack.Item;
    private Control _messageContainer = null!;
    private Label _messageLabel = null!;

    protected override void SetupData(object? data)
    {
        if (data is not ItemStack itemStack)
            return;
        _itemStack = itemStack;
        _actionEffect = _actionEffectDB.GetEffect(Item.UseData.ActionEffect)!;
        _enemies = _areaScene?.GetAllActorBodiesWithinView()
            .Where(x => x.Role == (int)ActorRole.Enemy)
            .ToList() ?? new();
    }

    protected override void CustomSetup()
    {
        DisplayOptions();
    }

    protected override async Task AnimateOpenAsync()
    {
        if (Menu != null)
            await Menu.HideInactiveSubMenus();

        if (_areaScene == null)
            return;
        _areaScene.ColorAdjustment.Brightness = -0.5f;

        foreach (AActorBody enemy in _enemies)
            _areaScene.MoveToFocusLayer(enemy);
    }

    protected override void OnCloseSubMenu()
    {
        _ = Menu.ShowInactiveSubMenus();
        _areaScene?.ColorAdjustment.Reset();
        foreach (AActorBody enemy in _enemies)
            _areaScene?.MoveToActorContainer(enemy);
    }

    protected override void OnSelectPressed()
    {
        _ = HandleUse(CurrentContainer.FocusedItem);
    }

    protected override void SetNodeReferences()
    {
        _messageContainer = GetNode<Control>("%MessageContainer");
        _messageLabel = GetNode<Label>("%MessageLabel");
        _enemyContainer = GetNode<OptionContainer>("%EnemyOptions");
        AddContainer(_enemyContainer);

        if (_itemStack == null)
            return;
        if (_actionEffect.TargetType == (int)TargetType.EnemyAll)
        {
            _enemyContainer.AllOptionEnabled = true;
            _enemyContainer.SingleOptionsEnabled = false;
        }
    }

    private void DisplayOptions()
    {
        _enemyContainer.ClearOptionItems();
        foreach (AActorBody actorBody in _enemies)
        {
            var option = _noDisplayOptionScene.Instantiate<NoDisplayOption>();
            _enemyContainer.AddOption(option);
            option.OptionData = actorBody;
            Vector2 bodyPos = actorBody.GetGlobalTransformWithCanvas().Origin;
            bodyPos.X -= 5;
            option.GlobalPosition = bodyPos;
        }

        _messageContainer.Visible = _enemies.Count == 0;
        _messageLabel.Text = "No enemies found!";
    }

    private async Task HandleUse(OptionItem? optionItem)
    {
        IEnumerable<OptionItem> selectedItems;
        if (_enemyContainer.AllOptionEnabled)
            selectedItems = _enemyContainer.GetSelectedItems();
        else if (optionItem != null)
            selectedItems = new OptionItem[] { optionItem };
        else
            selectedItems = Array.Empty<OptionItem>();

        if (selectedItems.All(x => x.Disabled))
            return;

        List<AActor> targets = new();
        foreach (OptionItem item in selectedItems)
        {
            if (item.OptionData is not AActorBody actorBody)
                continue;
            if (actorBody.Actor == null)
                continue;
            targets.Add(actorBody.Actor);
        }
        AActor? user = _gameSession.MainParty?.Actors.First();

        _areaScene?.ColorAdjustment.Reset();
        foreach (AActorBody enemy in _enemies)
            _areaScene?.MoveToActorContainer(enemy);
        await CloseMenuAsync();

        _inventory.RemoveItem(_itemStack);
        if (_actionEffect.IsActionSequence)
            _gameSession.StartActionSequence(targets);
        await _actionEffect.Use(user, targets, (int)ActionType.Item, Item.UseData.Value1, Item.UseData.Value2);
        if (_actionEffect.IsActionSequence)
            _gameSession.StopActionSequence();
    }
}
