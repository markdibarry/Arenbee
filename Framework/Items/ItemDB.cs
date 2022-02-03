using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors.Stats;
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
            return s_items.FirstOrDefault(item => item.Id.Equals(id));
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
                    StatModifiers = new StatModifier[]
                    {
                        new StatModifier()
                        {
                            StatType = StatType.Attack,
                            Effect = ModifierEffect.Add,
                            Value = 2
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
