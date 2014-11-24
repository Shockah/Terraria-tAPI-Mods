using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.Base;
using Shockah.InvTweaks;

namespace Shockah.InvTweaks.SlotActions
{
	public class QuickMoveAction : SlotAction
	{
		public QuickMoveAction(MButton mouseButton, KKeys kboardKeys) : base(mouseButton, kboardKeys) { }

		public override bool Applies(ItemSlot slot, bool release)
		{
			return !slot.MyItem.IsBlank() && Main.localPlayer.chestItems != null && release;
		}

		public override bool Call(ItemSlot slot, bool release)
		{
			if (slot.type == "Inventory" || slot.type == "Coin" || slot.type == "Ammo")
			{
				Item myItem = slot.MyItem;
				SBase.PutItem(ref myItem, Main.localPlayer.chestItems);
				Main.PlaySound(7, -1, -1, 1);
				slot.MyItem = myItem;
				return true;
			}
			else if (slot.type == "Chest")
			{
				Item myItem = slot.MyItem;
				if (!myItem.IsBlank() && myItem.type >= 71 && myItem.type <= 74) SBase.PutItem(ref myItem, Main.localPlayer.inventory, 50, 53);
				if (!myItem.IsBlank() && myItem.ammo > 0 && !myItem.notAmmo) SBase.PutItem(ref myItem, Main.localPlayer.inventory, 54, 57);
				if (!myItem.IsBlank()) SBase.PutItem(ref myItem, Main.localPlayer.inventory, 0, 49);
				Main.PlaySound(7, -1, -1, 1);
				slot.MyItem = myItem;
				return true;
			}
			return base.Call(slot, release);
		}
	}
}