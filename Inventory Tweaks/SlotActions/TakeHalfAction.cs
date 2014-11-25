using System;
using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.Base;
using Shockah.InvTweaks;

namespace Shockah.InvTweaks.SlotActions
{
	public class TakeHalfAction : SlotAction
	{
		public override bool Applies(ItemSlot slot, bool release)
		{
			return !slot.MyItem.IsBlank() && slot.MyItem.maxStack > 1 && Main.mouseItem.IsBlank() && slot.AllowsItem(Main.mouseItem) && release;
		}

		public override bool Call(ItemSlot slot, bool release)
		{
			Main.mouseItem = (Item)slot.MyItem.Clone();
			Main.mouseItem.stack = Math.Max(slot.MyItem.stack / 2, 1);
			slot.MyItem.stack -= Main.mouseItem.stack;
			Main.PlaySound(7, -1, -1, 1);
			return true;
		}
	}
}