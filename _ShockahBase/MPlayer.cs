using System;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class MPlayer : ModPlayer
	{
		protected static void CopyFromItem(Item target, Item source)
		{
			target.netDefaults(source.netID);
			target.stack = source.stack;
		}
		
		protected Item[] inventory = null, armor = null, dye = null;

		public override void PostUpdate()
		{
			if (Main.netMode == 2 || player.whoAmI != Main.myPlayer) return;
			SEvent<Player, string, int, Item, Item> ev = ((MBase)modBase).handler.events["InventoryChanged"];
			if (ev.Count == 0) return;

			bool wasNull = false;
			if (inventory == null)
			{
				inventory = new Item[player.inventory.Length - 1]; //without the mouse item in the last (fake) slot
				for (int i = 0; i < inventory.Length; i++) inventory[i] = new Item();
				armor = new Item[player.armor.Length];
				for (int i = 0; i < armor.Length; i++) armor[i] = new Item();
				dye = new Item[player.dye.Length];
				for (int i = 0; i < dye.Length; i++) dye[i] = new Item();
				wasNull = true;
			}

			for (int i = 0; i < inventory.Length; i++)
			{
				if (!player.inventory[i].IsTheSameAs(inventory[i]) || player.inventory[i].stack != inventory[i].stack || wasNull)
				{
					if (!wasNull) ev.Call(player, "Inventory", i, inventory[i], player.inventory[i]);
					CopyFromItem(inventory[i], player.inventory[i]);
				}
			}
			for (int i = 0; i < armor.Length; i++)
			{
				if (!player.armor[i].IsTheSameAs(armor[i]) || player.armor[i].stack != armor[i].stack || wasNull)
				{
					if (!wasNull) ev.Call(player, "Armor", i, armor[i], player.armor[i]);
					CopyFromItem(armor[i], player.armor[i]);
				}
			}
			for (int i = 0; i < dye.Length; i++)
			{
				if (!player.dye[i].IsTheSameAs(dye[i]) || player.dye[i].stack != dye[i].stack || wasNull)
				{
					if (!wasNull) ev.Call(player, "Dye", i, dye[i], player.dye[i]);
					CopyFromItem(dye[i], player.dye[i]);
				}
			}
		}
	}
}
