using System.Collections.Generic;
using GameCore.ActionEffects;
using GameCore.Items;
using GameCore.Statistics;

namespace Arenbee.Items;

public class ItemDB : ItemDBBase
{
    protected override void BuildDB(List<ItemBase> items)
    {
        BuildWeapons(items);
        BuildHeadGear(items);
        BuildShirt(items);
        BuildPants(items);
        BuildFootwear(items);
        BuildAccessories(items);
        BuildRestorative(items);
        BuildKey(items);
    }

    private static void BuildWeapons(List<ItemBase> items)
    {
        items.Add(new("HockeyStick", ItemCategoryIds.Weapon)
        {
            DisplayName = "Hockey Stick",
            Description = "Perfect for slap-shots.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 10,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Attack, ModOperator.Add, 1),
                new(StatType.ElementOff, (int)ElementType.Earth, ModOperator.Add, 2)
            }
        });

        items.Add(new("MetalHockeyStick", ItemCategoryIds.Weapon)
        {
            DisplayName = "Metal Hockey Stick",
            Description = "It's not sharp. Don't worry!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 10,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Attack, ModOperator.Add, 1),
                new(StatType.ElementOff, (int)ElementType.Water, ModOperator.Add, 2)
            }
        });

        items.Add(new("Wand", ItemCategoryIds.SubWeapon)
        {
            DisplayName = "Magic Wand",
            Description = "Boom! Blast!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 10,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.MagicAttack, ModOperator.Add, 1),
                new(StatType.ElementOff, (int)ElementType.Fire, ModOperator.Add, 2)
            }
        });
    }

    private static void BuildHeadGear(List<ItemBase> items)
    {
        items.Add(new("CheeseHat", ItemCategoryIds.Headgear)
        {
            DisplayName = "Cheese Hat",
            Description = "A Wisconsin favorite!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.MagicAttack, ModOperator.Add, 1)
            }
        });

        items.Add(new("FaceMask", ItemCategoryIds.Headgear)
        {
            DisplayName = "Face Mask",
            Description = "Cheap but effective.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.StatusEffectDef, (int)StatusEffectType.Poison, ModOperator.Add, 20)
            }
        });

        items.Add(new("RamenBoushi", ItemCategoryIds.Headgear)
        {
            DisplayName = "Ramen Boushi",
            Description = "Vital for any fisherman!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.MagicDefense, ModOperator.Add, 1),
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, -1)
            }
        });

        items.Add(new("SunGlasses", ItemCategoryIds.Headgear)
        {
            DisplayName = "Sun Glasses",
            Description = "To be worn exclusively at night.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.ElementDef, (int)ElementType.Dark, ModOperator.Add, ElementDef.Resist)
            }
        });
    }

    private static void BuildShirt(List<ItemBase> items)
    {
        items.Add(new("ClemsonHoodie", ItemCategoryIds.Shirt)
        {
            DisplayName = "Clemson Hoodie",
            Description = "Football is a sport!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 1),
                new(StatType.Attribute, (int)AttributeType.MagicAttack, ModOperator.Add, 1)
            }
        });

        items.Add(new("MotleyCrueTee", ItemCategoryIds.Shirt)
        {
            DisplayName = "Motley Crue Tshirt",
            Description = "Shout at the devil!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 1)
            }
        });
    }

    private static void BuildPants(List<ItemBase> items)
    {
        items.Add(new("JNCOJeans", ItemCategoryIds.Pants)
        {
            DisplayName = "JNCO Jeans",
            Description = "Watch out for puddles.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 1),
                new(StatType.ElementDef, (int)ElementType.Water, ModOperator.Add, ElementDef.Weak)
            }
        });

        items.Add(new("ZubazPants", ItemCategoryIds.Pants)
        {
            DisplayName = "Zubaz Pants",
            Description = "They're like the 80's but in the 90's.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 14,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 2),
            }
        });
    }

    private static void BuildFootwear(List<ItemBase> items)
    {
        items.Add(new("Vibrams", ItemCategoryIds.Footwear)
        {
            DisplayName = "Vibrams",
            Description = "They feel as cool as they look!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 2)
            }
        });

        items.Add(new("Uggs", ItemCategoryIds.Footwear)
        {
            DisplayName = "Uggs",
            Description = "Lets get white-girl wasted!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 1),
                new(StatType.Attribute, (int)AttributeType.MagicDefense, ModOperator.Add, 1),
                new(StatType.ElementDef, (int)ElementType.Water, ModOperator.Add, ElementDef.Resist)
            }
        });

        items.Add(new("SnakeskinShoes", ItemCategoryIds.Footwear)
        {
            DisplayName = "Snakeskin Shoes",
            Description = "Goro Majima approved!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new (StatType.ElementDef, (int)ElementType.Water, ModOperator.Add, ElementDef.Resist),
                new (StatType.StatusEffectDef, (int)StatusEffectType.Poison, ModOperator.Add, 100)
            }
        });
    }

    private static void BuildAccessories(List<ItemBase> items)
    {
        items.Add(new("FriendshipBracelet", ItemCategoryIds.Accessory)
        {
            DisplayName = "Friendship Bracelet",
            Description = "Because I love you 5-ever.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.Attribute, (int)AttributeType.MaxHP, ModOperator.Add, 10)
            }
        });

        items.Add(new("SnakeRing", ItemCategoryIds.Accessory)
        {
            DisplayName = "Snake Ring",
            Description = "Why would you wear this?",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new(StatType.StatusEffect, (int)StatusEffectType.Poison, ModOperator.Add, 1)
            }
        });

        items.Add(new("MoodRing", ItemCategoryIds.Accessory)
        {
            DisplayName = "Mood Ring",
            Description = "It's just black.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new (StatType.Attribute, (int)AttributeType.MagicDefense, ModOperator.Add, 1),
                new (StatType.ElementDef, (int)ElementType.Dark, ModOperator.Add, ElementDef.Resist)
            }
        });

        items.Add(new("FingerlessGloves", ItemCategoryIds.Accessory)
        {
            DisplayName = "Fingerless Gloves",
            Description = "That bohemian look.",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new (StatType.Attribute, (int)AttributeType.Defense, ModOperator.Add, 1)
            }
        });

        items.Add(new("OvenMitts", ItemCategoryIds.Accessory)
        {
            DisplayName = "Oven Mitts",
            Description = "Keeps your hands burn-free for up to 5 seconds!",
            MaxStack = 9,
            IsSellable = false,
            IsDroppable = false,
            Price = 15,
            Modifiers = new Modifier[]
            {
                new (StatType.ElementDef, (int)ElementType.Fire, ModOperator.Add, ElementDef.Resist),
                new (StatType.StatusEffectDef, (int)StatusEffectType.Burn, ModOperator.Add, 10)
            }
        });
    }

    private static void BuildRestorative(List<ItemBase> items)
    {
        items.Add(new("Potion", ItemCategoryIds.Restorative)
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

        items.Add(new("FlavorAid", ItemCategoryIds.Restorative)
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

        items.Add(new("GeneSupreme", ItemCategoryIds.Restorative)
        {
            DisplayName = "Gene Supreme",
            Description = "Restores a good bit of HP, and you get your name on the wall if you finish it!",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 30
        });

        items.Add(new("SuperDonut", ItemCategoryIds.Restorative)
        {
            DisplayName = "Super Donut",
            Description = "Restores a bit of MP. Even more when microwaved.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("TurboEther", ItemCategoryIds.Restorative)
        {
            DisplayName = "Turbo Ether",
            Description = "Restores a good bit of MP.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 30
        });

        items.Add(new("Elixer", ItemCategoryIds.Restorative)
        {
            DisplayName = "Elixer",
            Description = "Restores 30% of your HP & MP.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 90
        });

        items.Add(new("Antidote", ItemCategoryIds.Restorative)
        {
            DisplayName = "Antidote",
            Description = "Removes Poison status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("Aloe", ItemCategoryIds.Restorative)
        {
            DisplayName = "Aloe",
            Description = "Removes Burn status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("LifeAlert", ItemCategoryIds.Restorative)
        {
            DisplayName = "Life Alert",
            Description = "Auto-Restores KO once.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("EscapeRope", ItemCategoryIds.Restorative)
        {
            DisplayName = "Escape Rope",
            Description = "Returns you to the entrance of a dungeon.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("EyeDrops", ItemCategoryIds.Restorative)
        {
            DisplayName = "Eye Drops",
            Description = "Removes the Blind status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("ThroatSpray", ItemCategoryIds.Restorative)
        {
            DisplayName = "Throat Spray",
            Description = "Removes the Silent status.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("SubwaySandwich", ItemCategoryIds.Restorative)
        {
            DisplayName = "Subway Sandwich",
            Description = "Baked fresh, yet somehow tastes old.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });

        items.Add(new("IPA", ItemCategoryIds.Restorative)
        {
            DisplayName = "IPA",
            Description = "People say it tastes good. It doesn't.",
            MaxStack = 9,
            IsSellable = true,
            IsDroppable = true,
            Price = 10
        });
    }

    private static void BuildKey(List<ItemBase> items)
    {
        items.Add(new("BunnyNugget", ItemCategoryIds.Key)
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
