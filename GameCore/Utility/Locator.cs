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
    private static AAudioController s_audioController = null!;
    private static AEquipmentSlotCategoryDB s_equipmentSlotCategoryDB = null!;
    private static AItemDB s_itemDB = null!;
    private static AItemCategoryDB s_itemCategoryDB = null!;
    private static StatusEffectDBBase s_statusEffectDB = null!;
    private static ActionEffectDBBase s_actionEffectDB = null!;
    private static TransitionControllerBase s_transitionController = null!;

    public static ActionEffectDBBase ActionEffectDB => s_actionEffectDB;
    public static AActorDataDB ActorDataDB => s_actorDataDB;
    public static AAudioController Audio => s_audioController;
    public static AEquipmentSlotCategoryDB EquipmentSlotCategoryDB => s_equipmentSlotCategoryDB;
    public static AItemDB ItemDB => s_itemDB;
    public static AItemCategoryDB ItemCategoryDB => s_itemCategoryDB;
    public static AGameRoot Root => s_gameRoot;
    public static AGameSession? Session => s_gameRoot.GameSession;
    public static StatusEffectDBBase StatusEffectDB => s_statusEffectDB;
    public static TransitionControllerBase TransitionController => s_transitionController;

    public static void ProvideActionEffectDB(ActionEffectDBBase actionEffectDB)
    {
        s_actionEffectDB = actionEffectDB;
    }

    public static void ProvideActorDataDB(AActorDataDB actorDataDB)
    {
        s_actorDataDB = actorDataDB;
    }

    public static void ProvideAudioController(AAudioController audioController)
    {
        if (GodotObject.IsInstanceValid(s_audioController))
            s_audioController.Free();
        s_audioController = audioController;
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

    public static void ProvideStatusEffectDB(StatusEffectDBBase statusEffectDB)
    {
        s_statusEffectDB = statusEffectDB;
    }

    public static void ProvideTransitionController(TransitionControllerBase transitionController)
    {
        s_transitionController = transitionController;
    }
}
