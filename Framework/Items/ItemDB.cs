using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public static class ItemDB
    {
        static ItemDB()
        {
            BuildDB();
        }
        private static readonly List<Item> s_items = new List<Item>();

        public static Item GetItem(string id)
        {
            return s_items.Find(item => item.Id.Equals(id));
        }

        public static IEnumerable<Item> GetItemsByType(ItemType itemType)
        {
            return s_items.Where(item => item.ItemType.Equals(itemType));
        }

        private static void BuildDB()
        {
            s_items.Add(new Item()
            {
                Id = "HockeyStick",
                DisplayName = "Hockey Stick",
                ItemType = ItemType.Weapon,
                Description = "Perfect for slap-shots.",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 10,
                ItemStats = new ItemStats()
                {
                    ActionElement = Element.Earth,
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Attack,
                            Effect = ModifierEffect.Add,
                            Value = 2
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "ClemsonHoodie",
                DisplayName = "Clemson Hoodie",
                ItemType = ItemType.Shirt,
                Description = "Football is a sport!",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Defense,
                            Effect = ModifierEffect.Add,
                            Value = 2
                        },
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.MagicAttack,
                            Effect = ModifierEffect.Subtract,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "FriendshipBracelet",
                DisplayName = "Friendship Bracelet",
                ItemType = ItemType.Accessory,
                Description = "Because I love you 5-ever.",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.MagicDefense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "MoodRing",
                DisplayName = "Mood Ring",
                ItemType = ItemType.Accessory,
                Description = "It's just black.",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.MagicDefense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "Vibrams",
                DisplayName = "Vibrams",
                ItemType = ItemType.Footwear,
                Description = "They feel as cool as they look!",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Defense,
                            Effect = ModifierEffect.Add,
                            Value = 2
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "JNCOJeans",
                DisplayName = "JNCO Jeans",
                ItemType = ItemType.Pants,
                Description = "Watch out for puddles.",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Defense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "MotleyCrueTee",
                DisplayName = "Motley Crue Tshirt",
                ItemType = ItemType.Shirt,
                Description = "Shout at the devil!",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Defense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "CheeseHat",
                DisplayName = "Cheese Hat",
                ItemType = ItemType.Headgear,
                Description = "A Wisconsin favorite!",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.MagicAttack,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "Uggs",
                DisplayName = "Uggs",
                ItemType = ItemType.Footwear,
                Description = "Lets get white-girl wasted!",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Defense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        },
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.MagicDefense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "RamenBoushi",
                DisplayName = "Ramen Boushi",
                ItemType = ItemType.Headgear,
                Description = "Vital for any fisherman!",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.MagicDefense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "FingerlessGloves",
                DisplayName = "Fingerless Gloves",
                ItemType = ItemType.Accessory,
                Description = "That bohemian look.",
                MaxStack = 9,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 15,
                ItemStats = new ItemStats()
                {
                    AttributeModifiers = new AttributeModifier[]
                    {
                        new AttributeModifier()
                        {
                            AttributeType = AttributeType.Defense,
                            Effect = ModifierEffect.Add,
                            Value = 1
                        }
                    }
                }
            });

            s_items.Add(new Item()
            {
                Id = "Potion",
                DisplayName = "Potion",
                ItemType = ItemType.Restorative,
                Description = "Restores up to 10 of your HP.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10,
            });

            s_items.Add(new Item()
            {
                Id = "GeneSupreme",
                DisplayName = "Gene Supreme",
                ItemType = ItemType.Restorative,
                Description = "Restores up to 30 of your HP, and you get your name on the wall!",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 30,
            });

            s_items.Add(new Item()
            {
                Id = "SuperDonut",
                DisplayName = "Super Donut",
                ItemType = ItemType.Restorative,
                Description = "Restores up to 10 of your MP. Even more when microwaved.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "TurboEther",
                DisplayName = "Turbo Ether",
                ItemType = ItemType.Restorative,
                Description = "Restores up to 30 of your MP.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 30
            });

            s_items.Add(new Item()
            {
                Id = "Elixer",
                DisplayName = "Elixer",
                ItemType = ItemType.Restorative,
                Description = "Restores 30% of your HP & MP.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 90
            });

            s_items.Add(new Item()
            {
                Id = "Antidote",
                DisplayName = "Antidote",
                ItemType = ItemType.Restorative,
                Description = "Removes Poison status.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "Aloe",
                DisplayName = "Aloe",
                ItemType = ItemType.Restorative,
                Description = "Removes Burn status.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "LifeAlert",
                DisplayName = "Life Alert",
                ItemType = ItemType.Restorative,
                Description = "Auto-Restores KO once.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "EscapeRope",
                DisplayName = "Escape Rope",
                ItemType = ItemType.Restorative,
                Description = "Returns you to the entrance of a dungeon.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "EyeDrops",
                DisplayName = "Eye Drops",
                ItemType = ItemType.Restorative,
                Description = "Removes the Blind status.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "ThroatSpray",
                DisplayName = "Throat Spray",
                ItemType = ItemType.Restorative,
                Description = "Removes the Silent status.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "SubwaySandwich",
                DisplayName = "Subway Sandwich",
                ItemType = ItemType.Restorative,
                Description = "Baked fresh, yet somehow tastes old.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "IPA",
                DisplayName = "IPA",
                ItemType = ItemType.Restorative,
                Description = "People say it tastes good. It doesn't.",
                MaxStack = 9,
                IsUsableInMenu = true,
                IsUsableInField = true,
                IsSellable = true,
                IsDroppable = true,
                Price = 10
            });

            s_items.Add(new Item()
            {
                Id = "BunnyNugget",
                DisplayName = "Bunny Nugget",
                ItemType = ItemType.Key,
                Description = "Unseals the dreaded Di-a-blur.",
                MaxStack = 1,
                IsUsableInMenu = false,
                IsUsableInField = false,
                IsSellable = false,
                IsDroppable = false,
                Price = 800
            });
        }
    }
}
