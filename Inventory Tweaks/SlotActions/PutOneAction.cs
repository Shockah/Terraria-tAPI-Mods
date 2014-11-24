using System;
using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.Base;
using Shockah.InvTweaks;

namespace Shockah.InvTweaks.SlotActions
{
	public class PutOneAction : SlotAction
	{
		public PutOneAction(MButton mouseButton, KKeys kboardKeys) : base(mouseButton, kboardKeys) { }

		public override bool Applies(ItemSlot slot, bool release)
		{
			return (slot.MyItem.IsBlank() || slot.MyItem.IsTheSameAs(Main.mouseItem) && !Main.mouseItem.IsBlank() && slot.AllowsItem(Main.mouseItem) && release;
		}

		public override bool Call(ItemSlot slot, bool release)
		{
			if (slot.MyItem.IsBlank())
			{
				slot.MyItem = (Item)Main.mouseItem.Clone();
				slot.stack = 0;
			}
			slot.MyItem.stack++;
			Main.mouseItem.stack--;
			if (Main.mouseItem.IsBlank())
				Main.mouseItem.SetDefaults(0);
			Main.PlaySound(7, -1, -1, 1);
			return true;
		}
	}
}