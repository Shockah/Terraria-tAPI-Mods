using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	[GlobalMod] public class MItem : ModItem
	{
		public int lastMana = -1;

		[CallPriority(-10000f)] public override string OnAffixName(string currentName, string oldName)
		{
			if (InterfaceFCMItems.displayIds)
			{
				currentName = "[" + item.type + (item.netID != item.type ? "/" + item.netID : "") + "] " + currentName;
				InterfaceFCMItems.displayIds = false;
			}
			return currentName;
		}

		public override bool ConsumeAmmo(Player p)
		{
			MPlayer mp = p.GetSubClass<MPlayer>();
			if (mp.cheatUsage) return false;
			return base.ConsumeAmmo(p);
		}

		public override bool ConsumeItem(Player p)
		{
			MPlayer mp = p.GetSubClass<MPlayer>();
			if (mp.cheatTileUsage && (item.createTile >= 0 || item.createWall > 0)) return false;
			return base.ConsumeItem(p);
		}

		public override bool CanUse(Player p)
		{
			MPlayer mp = p.GetSubClass<MPlayer>();
			if (mp.cheatUsage)
			{
				lastMana = item.mana;
				item.mana = 0;
			}
			return base.CanUse(p);
		}
	}
}