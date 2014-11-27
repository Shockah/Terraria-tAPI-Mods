using Microsoft.Xna.Framework;
using System;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	[GlobalMod] public class MTileType : ModTileType
	{
		public static int nested = 0;
		public static uint[] cacheItems;

		public override bool PreKill(int x, int y, bool fail, bool effectsOnly, bool noItem)
		{
			SEvent<int, int, int, Item> ev = ((MBase)modBase).handler.events["TileLoot"];
			if (ev.Count == 0) return;

			if (nested == 0)
			{
				cacheItems = new uint[Main.item.Length - 1];
				for (int i = 0; i < cacheItems.Length; i++)
				{
					if (Main.item[i].IsBlank()) continue;
					cacheItems[i] = Main.item[i].GetSubClass<MItem>().myId;
				}
			}
			nested++;
			return true;
		}

		public override void PostKill(int x, int y, bool fail, bool effectsOnly, bool noItem)
		{
			SEvent<int, int, int, Item> ev = ((MBase)modBase).handler.events["TileLoot"];
			if (ev.Count == 0) return;

			nested--;
			if (nested == 0)
			{
				for (int i = 0; i < cacheItems.Length; i++)
				{
					if (Main.item[i].IsBlank()) continue;
					if (Main.item[i].GetSubClass<MItem>().myId != cacheItems[i])
						ev.Call(x, y, i, Main.item[i]);
				}
			}
		}
	}
}