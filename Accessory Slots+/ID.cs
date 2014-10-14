using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.AccSlots
{
	public static class ID
	{
		public static Item[] ITEM_SLOT = new Item[MNPC.MAX_SHOP_SLOTS];
		
		public static void Fill(MBase modBase)
		{
			for (int i = 0; i < ITEM_SLOT.Length; i++)
			{
				ITEM_SLOT[i] = ItemDef.byName[modBase.mod.InternalName + ":ExtraSlot" + i];
			}
		}
	}
}