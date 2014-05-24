﻿using LitJson;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public static class SBase
	{
		public static SEvent<Action<bool>> EventMenuStateChange = new SEvent<Action<bool>>();
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

        public static bool PutItem(ref Item item, Item[] items)
        {
            if (item.IsBlank()) return false;
            foreach (Item item2 in items)
            {
                if (!item2.IsBlank() && item2.IsTheSameAs(item) && item2.stack < item2.maxStack)
                {
                    int diff = Math.Min(item2.maxStack - item2.stack, item.stack);
                    item2.stack += diff;
                    item.stack -= diff;
                    if (item.IsBlank())
                    {
                        item = new Item();
                        return true;
                    }
                }
            }
            for (int item2id = 0; item2id < items.Length; item2id++)
            {
                if (items[item2id].IsBlank())
                {
                    items[item2id] = item;
                    item = new Item();
                    return true;
                }
            }
            return false;
        }

		public static JsonData JsonObject(params object[] objs)
		{
			if (objs.Length % 2 == 1) throw new ArgumentException();
			JsonData j = JsonMapper.ToObject("{}");
			for (int i = 0; i < objs.Length; i += 2) j[(string)objs[i]] = ToJsonData(objs[i + 1]);
			return j;
		}
		public static JsonData JsonArray(params object[] objs)
		{
			JsonData j = JsonMapper.ToObject("[]");
			for (int i = 0; i < objs.Length; i++) j.Add(objs[i]);
			return j;
		}
		private static JsonData ToJsonData(object obj)
		{
			if (obj == null) return null;
			if (obj is JsonData) return (JsonData)obj;
			return new JsonData(obj);
		}
	}
}