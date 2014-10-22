using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.MyFavWeapon
{
	public class MInterface : ModInterface
	{
		public override bool PreItemSlotLeftClick(ItemSlot slot, ref bool release)
		{
			if (!Main.mouseItem.IsBlank() && Main.mouseItem.name == modBase.mod.InternalName + ":SymbolOfLove")
			{
				if (slot.type == "Inventory")
				{
					Item item = slot.MyItem;
					if (!item.IsBlank() && !item.accessory && item.maxStack == 1 && item.damage > 0)
					{
						MItem mi = item.GetSubClass<MItem>();
						if (mi.name == null)
						{
							Main.mouseItem.type = 0;
							Main.mouseItem.name = "";
							Main.mouseItem.stack = 0;
							
							item.value *= 2;
							mi.Backup();
							item.prefix = Prefix.None;
							mi.name = "Favorite Weapon";
							Main.PlaySound(2, -1, -1, 37);
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}