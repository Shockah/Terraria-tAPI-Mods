using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class SBase
	{
		public static SEvent<Action<Player, string, int, Item, Item>> EventInventoryChange = new SEvent<Action<Player, string, int, Item, Item>>();
		public static SEvent<Action<STooltip, Rectangle>> EventPreSTooltipDraw = new SEvent<Action<STooltip, Rectangle>>();
		public static SEvent<Func<STooltip>> EventSTooltipDraw = new SEvent<Func<STooltip>>();
		public static SEvent<Func<NPC, bool>> EventIsBoss = new SEvent<Func<NPC, bool>>();
		public static SEvent<Action<NPC, Item>> EventNPCLoot = new SEvent<Action<NPC, Item>>();
		public static SEvent<Action<Point, Item>> EventTileLoot = new SEvent<Action<Point, Item>>();

		public static void InsertAfter<T>(List<T> list, Func<T, bool> comparator, T add)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (comparator(list[i]))
				{
					if (i != list.Count - 1)
					{
						list.Insert(i + 1, add);
						return;
					}
				}
			}
			list.Add(add);
		}

		public static bool IsBoss(NPC npc)
		{
			foreach (Func<NPC, bool> h in EventIsBoss) if (h(npc)) return true;
			if (npc.boss) return true;
			if (npc.type >= 13 && npc.type <= 15) return true; //Eater of Worlds
			if (npc.type >= 134 && npc.type <= 136) return true; //The Destroyer
			return false;
		}
	}
}