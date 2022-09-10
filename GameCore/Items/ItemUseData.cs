using GameCore.ActionEffects;

namespace GameCore.Items;

public class ItemUseData
{
    public ActionEffectType ActionEffect { get; set; }
    public ItemUseType UseType { get; init; }
    public int Value1 { get; set; }
    public int Value2 { get; set; }
}

public enum ItemUseType
{
    None,
    Self,
    PartyMember,
    PartyMemberAll,
    Enemy,
    EnemyRadius,
    EnemyCone,
    EnemyAll,
    OtherClose,
    Other
}
