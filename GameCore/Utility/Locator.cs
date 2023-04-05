using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Audio;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace GameCore.Utility;

public static class Locator
{
    private static AGameRoot s_gameRoot = null!;
    private static AActorDataDB s_actorDataDB = null!;
    private static IConditionEventFilterFactory s_conditionEventFilterFactory = null!;
    private static AEquipmentSlotCategoryDB s_equipmentSlotCategoryDB = null!;
    private static AItemDB s_itemDB = null!;
    private static AItemCategoryDB s_itemCategoryDB = null!;
    private static AStatTypeDB s_statTypeDB = null!;
    private static AStatusEffectDB s_statusEffectDB = null!;
    private static IStatusEffectModifierFactory s_statusEffectModifierFactory = null!;
    private static ActionEffectDBBase s_actionEffectDB = null!;

    public static ActionEffectDBBase ActionEffectDB => s_actionEffectDB;
    public static AActorDataDB ActorDataDB => s_actorDataDB;
    public static AAudioController Audio => s_gameRoot.AudioController;
    public static IConditionEventFilterFactory ConditionEventFilterFactory => s_conditionEventFilterFactory;
    public static AEquipmentSlotCategoryDB EquipmentSlotCategoryDB => s_equipmentSlotCategoryDB;
    public static AItemDB ItemDB => s_itemDB;
    public static AItemCategoryDB ItemCategoryDB => s_itemCategoryDB;
    public static AGameRoot Root => s_gameRoot;
    public static AGameSession? Session => s_gameRoot.GameSession;
    public static AStatTypeDB StatTypeDB => s_statTypeDB;
    public static AStatusEffectDB StatusEffectDB => s_statusEffectDB;
    public static IStatusEffectModifierFactory StatusEffectModifierFactory => s_statusEffectModifierFactory;
    public static TransitionControllerBase TransitionController => s_gameRoot.TransitionController;

    public static void ProvideActionEffectDB(ActionEffectDBBase actionEffectDB)
    {
        s_actionEffectDB = actionEffectDB;
    }

    public static void ProvideActorDataDB(AActorDataDB actorDataDB)
    {
        s_actorDataDB = actorDataDB;
    }

    public static void ProvideConditionEventFilterFactory(IConditionEventFilterFactory factory)
    {
        s_conditionEventFilterFactory = factory;
    }

    public static void ProvideEquipmentSlotCategoryDB(AEquipmentSlotCategoryDB equipmentSlotCategoryDB)
    {
        s_equipmentSlotCategoryDB = equipmentSlotCategoryDB;
    }

    public static void ProvideItemDB(AItemDB itemDB)
    {
        s_itemDB = itemDB;
    }

    public static void ProvideItemCategoryDB(AItemCategoryDB itemCategoryDB)
    {
        s_itemCategoryDB = itemCategoryDB;
    }

    public static void ProvideGameRoot(AGameRoot gameRoot)
    {
        if (GodotObject.IsInstanceValid(s_gameRoot))
            s_gameRoot.Free();
        s_gameRoot = gameRoot;
    }

    public static void ProvideStatTypeDB(AStatTypeDB statTypeDB)
    {
        s_statTypeDB = statTypeDB;
    }

    public static void ProvideStatusEffectDB(AStatusEffectDB statusEffectDB)
    {
        s_statusEffectDB = statusEffectDB;
    }

    public static void ProvideStatusEffectModifierFactory(IStatusEffectModifierFactory factory)
    {
        s_statusEffectModifierFactory = factory;
    }
}
