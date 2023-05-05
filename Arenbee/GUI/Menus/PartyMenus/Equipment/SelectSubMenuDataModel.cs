using Arenbee.Actors;
using GameCore.Items;

namespace Arenbee.GUI.Menus.PartyMenus.Equipment;

public record SelectSubMenuDataModel(EquipmentSlot Slot, Actor Actor, int Margin);
