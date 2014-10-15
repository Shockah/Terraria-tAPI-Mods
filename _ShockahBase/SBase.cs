using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public static class SBase
	{
		public static SEvent<Action<bool>> EventMenuStateChange;
		public static SEvent<Action<Player, string, int, Item, Item>> EventInventoryChange;
		public static SEvent<Action<SpriteBatch, STooltip, Rectangle>> EventPreSTooltipDraw, EventPostSTooltipDraw;
		public static SEvent<Func<STooltip>> EventSTooltipDraw;
		public static SEvent<Func<NPC, bool>> EventIsBoss, EventRequiresAttaching, EventUnsafeSpawn;
		public static SEvent<Action<NPC, Item>> EventNPCLoot;
		public static SEvent<Action<Point, Item>> EventTileLoot;
		public static string tip = null;

		internal static List<int> noTimerBuffs = new List<int>(new int[] {19, 27, 28, 34, 37, 38, 40, 41, 42, 43, 45, 49, 60, 62, 64, 67, 68, 81, 82, 83, 90});

		static SBase()
		{
			Clear();
		}

		internal static void Clear()
		{
			EventMenuStateChange = new SEvent<Action<bool>>();
			EventInventoryChange = new SEvent<Action<Player, string, int, Item, Item>>();
			EventPreSTooltipDraw = new SEvent<Action<SpriteBatch, STooltip, Rectangle>>();
			EventPostSTooltipDraw = new SEvent<Action<SpriteBatch, STooltip, Rectangle>>();
			EventSTooltipDraw = new SEvent<Func<STooltip>>();
			EventIsBoss = new SEvent<Func<NPC, bool>>();
			EventRequiresAttaching = new SEvent<Func<NPC, bool>>();
			EventUnsafeSpawn = new SEvent<Func<NPC, bool>>();
			EventNPCLoot = new SEvent<Action<NPC, Item>>();
			EventTileLoot = new SEvent<Action<Point, Item>>();
		}

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
		public static bool RequiresAttaching(NPC npc)
		{
			foreach (Func<NPC, bool> h in EventRequiresAttaching) if (h(npc)) return true;
			if (npc.type == 43 || npc.type == 56 || npc.type == 101 || npc.type == 175 || npc.type == 259 || npc.type == 260) return true;
			return false;
		}
		public static bool IsUnsafeToSpawn(NPC npc)
		{
			foreach (Func<NPC, bool> h in EventUnsafeSpawn) if (h(npc)) return true;
			if (npc.type == 263) return true;
			return false;
		}

		public static bool BuffHasTimer(int type)
		{
			if (Main.buffNoTimeDisplay[type]) return false;
			if (Main.vanityPet[type] || Main.lightPet[type]) return false;
			if (noTimerBuffs.Contains(type)) return false;
			if (!Main.localPlayer.honeyWet && type == 48) return true;
			return true;
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

		public static BinBuffer CopyFurther(BinBuffer source)
		{
			int pos = source.Pos;
			BinBuffer bb = new BinBuffer();
			bb.Write(source);
			bb.Pos = 0;
			source.Pos = pos;
			return bb;
		}
	}
}