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
			if (nested == 0)
			{
				cacheItems = new uint[Main.item.Length - 1];
				for (int i = 0; i < cacheItems.Length; i++)
				{
					if (Main.item[i].IsBlank()) continue;
					MItem mitem = Main.item[i].GetSubClass<MItem>();
					if (mitem == null) continue;
					cacheItems[i] = mitem.myId;
				}
			}
			nested++;
			return true;
		}

		public override void PostKill(int x, int y, bool fail, bool effectsOnly, bool noItem)
		{
			nested--;
			if (nested == 0)
			{
				for (int i = 0; i < cacheItems.Length; i++)
				{
					if (Main.item[i].IsBlank()) continue;
					MItem mitem = Main.item[i].GetSubClass<MItem>();
					if (mitem == null) continue;
					if (mitem.myId != cacheItems[i])
					{
						foreach (Action<Point, Item> h in SBase.EventTileLoot) h(new Point(x, y), Main.item[i]);
					}
				}
			}
		}
	}
}