using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.AccSlots
{
	[GlobalMod] public class MItem : ModItem
	{
		public override bool CanEquip(Player player, ItemSlot slot)
		{
			MPlayer mp = player.GetSubClass<MPlayer>();
			for (int i = 0; i < 5; i++)
			{
				if (player.armor[3 + i].IsTheSameAs(item)) return false;
				if (player.armor[11 + i].IsTheSameAs(item)) return false;
			}
			for (int i = 0; i < mp.currentExtraSlots; i++)
			{
				if (mp.extraItem[i].IsTheSameAs(item)) return false;
				if (mp.extraSocial[i].IsTheSameAs(item)) return false;
			}
			return base.CanEquip(player, slot);
		}
	}
}