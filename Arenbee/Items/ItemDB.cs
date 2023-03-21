using System.Collections.Generic;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Enums;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;

namespace Arenbee.Items;

public class ItemDB : AItemDB
{
    private static readonly IStatusEffectModifierFactory s_effectModFactory = Locator.StatusEffectModifierFactory;

    public ItemDB(AItemCategoryDB itemCategoryDB)
        : base(itemCategoryDB)
    {
    }

    protected override AItem[] BuildDB(AItemCategoryDB itemCategoryDB)
    {
        List<AItem> items = new();
        BuildWeapons(items, itemCategoryDB.GetCategory(ItemCategoryIds.Weapon)!);
        BuildHeadGear(items, itemCategoryDB.GetCategory(ItemCategoryIds.Headgear)!);
        BuildShirt(items, itemCategoryDB.GetCategory(ItemCategoryIds.Shirt)!);
        BuildPants(items, itemCategoryDB.GetCategory(ItemCategoryIds.Pants)!);
        BuildFootwear(items, itemCategoryDB.GetCategory(ItemCategoryIds.Footwear)!);
        BuildAccessories(items, itemCategoryDB.GetCategory(ItemCategoryIds.Accessory)!);
        BuildRestorative(items, itemCategoryDB.GetCategory(ItemCategoryIds.Restorative)!);
        BuildKey(items, itemCategoryDB.GetCategory(ItemCategoryIds.Key)!);
        return items.ToArray();
    }

    private static void BuildWeapons(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("HockeyStick", itemCategory)
        {
            DisplayName = "Hockey Stick",
            Description = "Perfect for slap-shots.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 10,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Attack, ModOp.Add, 1),
                new((int)StatType.AttackElement, ModOp.Add, (int)ElementType.Earth)
            }
        });

        items.Add(new Item("MetalHockeyStick", itemCategory)
        {
            DisplayName = "Metal Hockey Stick",
            Description = "It's not sharp. Don't worry!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 10,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Attack, ModOp.Add, 1),
                new((int)StatType.AttackElement, ModOp.Add, (int)ElementType.Water)
            }
        });

        items.Add(new Item("Wand", itemCategory)
        {
            DisplayName = "Magic Wand",
            Description = "Boom! Blast!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 10,
            Modifiers = new Modifier[]
            {
                new((int)StatType.MagicAttack, ModOp.Add, 1),
                new((int)StatType.AttackElement, ModOp.Add, (int)ElementType.Fire)
            }
        });
    }

    private static void BuildHeadGear(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("CheeseHat", itemCategory)
        {
            DisplayName = "Cheese Hat",
            Description = "A Wisconsin favorite!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.MagicAttack, ModOp.Add, 1)
            }
        });

        items.Add(new Item("FaceMask", itemCategory)
        {
            DisplayName = "Face Mask",
            Description = "Cheap but effective.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.PoisonResist, ModOp.Add, 20)
            }
        });

        items.Add(new Item("RamenBoushi", itemCategory)
        {
            DisplayName = "Ramen Boushi",
            Description = "Vital for any fisherman!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.MagicDefense, ModOp.Add, 1),
                new((int)StatType.Defense, ModOp.Subtract, 1)
            }
        });

        items.Add(new Item("SunGlasses", itemCategory)
        {
            DisplayName = "Sun Glasses",
            Description = "To be worn exclusively at night.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.DarkResist, ModOp.Add, ElementResist.Resist)
            }
        });
    }

    private static void BuildShirt(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("ClemsonHoodie", itemCategory)
        {
            DisplayName = "Clemson Hoodie",
            Description = "Football is a sport!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Defense, ModOp.Add, 1),
                new((int)StatType.MagicAttack, ModOp.Add, 1)
            }
        });

        items.Add(new Item("MotleyCrueTee", itemCategory)
        {
            DisplayName = "Motley Crue Tshirt",
            Description = "Shout at the devil!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Defense, ModOp.Add, 1)
            }
        });
    }

    private static void BuildPants(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("JNCOJeans", itemCategory)
        {
            DisplayName = "JNCO Jeans",
            Description = "Watch out for puddles.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Defense, ModOp.Add, 1),
                new((int)StatType.WaterResist, ModOp.Add, ElementResist.Weak)
            }
        });

        items.Add(new Item("ZubazPants", itemCategory)
        {
            DisplayName = "Zubaz Pants",
            Description = "They're like the 80's but in the 90's.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 14,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Defense, ModOp.Add, 2),
            }
        });
    }

    private static void BuildFootwear(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("Vibrams", itemCategory)
        {
            DisplayName = "Vibrams",
            Description = "They feel as cool as they look!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Defense, ModOp.Add, 2)
            }
        });

        items.Add(new Item("Uggs", itemCategory)
        {
            DisplayName = "Uggs",
            Description = "Lets get white-girl wasted!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.Defense, ModOp.Add, 1),
                new((int)StatType.MagicDefense, ModOp.Add, 1),
                new((int)StatType.WaterResist, ModOp.Add, ElementResist.Resist)
            }
        });

        items.Add(new Item("SnakeskinShoes", itemCategory)
        {
            DisplayName = "Snakeskin Shoes",
            Description = "Goro Majima approved!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new ((int)StatType.WaterResist, ModOp.Add, ElementResist.Resist),
                new ((int)StatType.PoisonResist, ModOp.Add, 100)
            }
        });
    }

    private static void BuildAccessories(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("FriendshipBracelet", itemCategory)
        {
            DisplayName = "Friendship Bracelet",
            Description = "Because I love you 5-ever.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new((int)StatType.MaxHP, ModOp.Add, 10)
            }
        });

        items.Add(new Item("SnakeRing", itemCategory)
        {
            DisplayName = "Snake Ring",
            Description = "Why would you wear this?",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                s_effectModFactory.GetStatusEffectModifier((int)StatusEffectType.Poison)
            }
        });

        items.Add(new Item("MoodRing", itemCategory)
        {
            DisplayName = "Mood Ring",
            Description = "It's just black.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new ((int)StatType.MagicDefense, ModOp.Add, 1),
                new ((int)StatType.DarkResist, ModOp.Add, ElementResist.Resist)
            }
        });

        items.Add(new Item("FingerlessGloves", itemCategory)
        {
            DisplayName = "Fingerless Gloves",
            Description = "That bohemian look.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new ((int)StatType.Defense, ModOp.Add, 1)
            }
        });

        items.Add(new Item("OvenMitts", itemCategory)
        {
            DisplayName = "Oven Mitts",
            Description = "Keeps your hands burn-free for up to 5 seconds!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new ((int)StatType.FireResist, ModOp.Add, ElementResist.Resist),
                new ((int)StatType.BurnResist, ModOp.Add, 10)
            }
        });
    }

    private static void BuildRestorative(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("Potion", itemCategory)
        {
            DisplayName = "Potion",
            Description = "Restores a bit of HP.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10,
            UseData = new()
            {
                ActionEffect = ActionEffectType.RestoreHP,
                UseType = ItemUseType.PartyMember,
                Value1 = 5
            }
        });

        items.Add(new Item("FlavorAid", itemCategory)
        {
            DisplayName = "Flavor Aid",
            Description = "Made to share!",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 30,
            UseData = new()
            {
                ActionEffect = ActionEffectType.RestoreHP,
                UseType = ItemUseType.PartyMemberAll,
                Value1 = 5
            }
        });

        items.Add(new Item("GeneSupreme", itemCategory)
        {
            DisplayName = "Gene Supreme",
            Description = "Restores a good bit of HP, and you get your name on the wall if you finish it!",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 30
        });

        items.Add(new Item("SuperDonut", itemCategory)
        {
            DisplayName = "Super Donut",
            Description = "Restores a bit of MP. Even more when microwaved.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("TurboEther", itemCategory)
        {
            DisplayName = "Turbo Ether",
            Description = "Restores a good bit of MP.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 30
        });

        items.Add(new Item("Elixer", itemCategory)
        {
            DisplayName = "Elixer",
            Description = "Restores 30% of your HP & MP.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 90
        });

        items.Add(new Item("Antidote", itemCategory)
        {
            DisplayName = "Antidote",
            Description = "Removes Poison status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("Aloe", itemCategory)
        {
            DisplayName = "Aloe",
            Description = "Removes Burn status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("LifeAlert", itemCategory)
        {
            DisplayName = "Life Alert",
            Description = "Auto-Restores KO once.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("EscapeRope", itemCategory)
        {
            DisplayName = "Escape Rope",
            Description = "Returns you to the entrance of a dungeon.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("EyeDrops", itemCategory)
        {
            DisplayName = "Eye Drops",
            Description = "Removes the Blind status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("ThroatSpray", itemCategory)
        {
            DisplayName = "Throat Spray",
            Description = "Removes the Silent status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("SubwaySandwich", itemCategory)
        {
            DisplayName = "Subway Sandwich",
            Description = "Baked fresh, yet somehow tastes old.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new Item("IPA", itemCategory)
        {
            DisplayName = "IPA",
            Description = "People say it tastes good. It doesn't.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });
    }

    private static void BuildKey(List<AItem> items, ItemCategory itemCategory)
    {
        items.Add(new Item("BunnyNugget", itemCategory)
        {
            DisplayName = "Bunny Nugget",
            Description = "Unseals the dreaded Di-a-blur.",
            MaxStack = 1,
            IsSellable = false,
            IsDroppable = false,
            Price = 800
        });
    }
}
