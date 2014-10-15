using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.AccSlots
{
	public class MItemExtraSlot : ModItem
	{
		public int Type
		{
			get
			{
				for (int i = 0; i < ID.ITEM_SLOT.Length; i++)
				{
					if (ID.ITEM_SLOT[i].type == item.type)
					{
						return i;
					}
				}
				throw new Exception("That wasn't supposed to happen.");
			}
		}
		
		public override bool? UseItem(Player player)
		{
			MPlayer mp = player.GetSubClass<MPlayer>();
			if (mp.boughtSlots[Type]) return false;
			mp.boughtSlots[Type] = true;
			int now = mp.currentSlots;
			foreach (Action<Player, int> h in MBase.EventUnlockSlot) h(player, now);
			return true;
		}
	}
}