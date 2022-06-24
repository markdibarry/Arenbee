using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.ActionEffects;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Items
{
    public class ItemDB : IItemDB
    {
        public ItemDB()
        {
            _items = new List<Item>();
            BuildDB();
        }

        private readonly List<Item> _items;

        public Item GetItem(string id)
        {
            return _items.Find(item => item.Id.Equals(id));
        }

        public IEnumerable<Item> GetItemsByType(ItemType itemType)
        {
            return _items.Where(item => item.ItemType.Equals(itemType));
        }

        private void BuildDB()
        {
            BuildWeapons();
            BuildHeadGear();
            BuildShirt();
            BuildPants();
            BuildFootwear();
            BuildAccessories();
            BuildRestorative();
            BuildKey();
        }

        private void BuildWeapons()
        {
            _items.Add(new("HockeyStick", ItemType.Weapon)
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

            _items.Add(new("MetalHockeyStick", ItemType.Weapon)
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

            _items.Add(new("Wand", ItemType.Weapon)
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

        private void BuildHeadGear()
        {
            _items.Add(new("CheeseHat", ItemType.Headgear)
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

            _items.Add(new("FaceMask", ItemType.Headgear)
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

            _items.Add(new("RamenBoushi", ItemType.Headgear)
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

            _items.Add(new("SunGlasses", ItemType.Headgear)
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

        private void BuildShirt()
        {
            _items.Add(new("ClemsonHoodie", ItemType.Shirt)
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

            _items.Add(new("MotleyCrueTee", ItemType.Shirt)
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

        private void BuildPants()
        {
            _items.Add(new Item("JNCOJeans", ItemType.Pants)
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

            _items.Add(new Item("ZubazPants", ItemType.Pants)
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

        private void BuildFootwear()
        {
            _items.Add(new("Vibrams", ItemType.Footwear)
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

            _items.Add(new("Uggs", ItemType.Footwear)
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

            _items.Add(new("SnakeskinShoes", ItemType.Footwear)
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

        private void BuildAccessories()
        {
            _items.Add(new("FriendshipBracelet", ItemType.Accessory)
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

            _items.Add(new("SnakeRing", ItemType.Accessory)
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

            _items.Add(new("MoodRing", ItemType.Accessory)
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

            _items.Add(new("FingerlessGloves", ItemType.Accessory)
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

            _items.Add(new("OvenMitts", ItemType.Accessory)
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

        private void BuildRestorative()
        {
            _items.Add(new Item("Potion", ItemType.Restorative)
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

            _items.Add(new Item("FlavorAid", ItemType.Restorative)
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

            _items.Add(new Item("GeneSupreme", ItemType.Restorative)
            {
                DisplayName = "Gene Supreme",
                Description = "Restores a good bit of HP, and you get your name on the wall if you finish it!",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 30
            });

            _items.Add(new Item("SuperDonut", ItemType.Restorative)
            {
                DisplayName = "Super Donut",
                Description = "Restores a bit of MP. Even more when microwaved.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("TurboEther", ItemType.Restorative)
            {
                DisplayName = "Turbo Ether",
                Description = "Restores a good bit of MP.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 30
            });

            _items.Add(new Item("Elixer", ItemType.Restorative)
            {
                DisplayName = "Elixer",
                Description = "Restores 30% of your HP & MP.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 90
            });

            _items.Add(new Item("Antidote", ItemType.Restorative)
            {
                DisplayName = "Antidote",
                Description = "Removes Poison status.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("Aloe", ItemType.Restorative)
            {
                DisplayName = "Aloe",
                Description = "Removes Burn status.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("LifeAlert", ItemType.Restorative)
            {
                DisplayName = "Life Alert",
                Description = "Auto-Restores KO once.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("EscapeRope", ItemType.Restorative)
            {
                DisplayName = "Escape Rope",
                Description = "Returns you to the entrance of a dungeon.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("EyeDrops", ItemType.Restorative)
            {
                DisplayName = "Eye Drops",
                Description = "Removes the Blind status.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("ThroatSpray", ItemType.Restorative)
            {
                DisplayName = "Throat Spray",
                Description = "Removes the Silent status.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("SubwaySandwich", ItemType.Restorative)
            {
                DisplayName = "Subway Sandwich",
                Description = "Baked fresh, yet somehow tastes old.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            _items.Add(new Item("IPA", ItemType.Restorative)
            {
                DisplayName = "IPA",
                Description = "People say it tastes good. It doesn't.",
                MaxStack = 9,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });
        }

        private void BuildKey()
        {
            _items.Add(new Item("BunnyNugget", ItemType.Key)
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
}
