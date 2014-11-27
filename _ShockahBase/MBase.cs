using System;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class MBase : ModBase
	{
		public readonly SExternalHandler handler = new SExternalHandler();
		protected bool wasInMenu = true;
		protected readonly List<int> noTimerBuffs = new List<int>(new int[] {19, 27, 28, 34, 37, 38, 40, 41, 42, 43, 45, 49, 60, 62, 64, 67, 68, 81, 82, 83, 90});

		public override void OnLoad()
		{
			handler.events["MenuStateChanged"] = new SEvent<bool>();
			handler.events["InventoryChanged"] = new SEvent<Player, string, int, Item, Item>();
			handler.events["NPCLoot"] = new SEvent<NPC, int, Item>();
			handler.events["TileLoot"] = new SEvent<int, int, int, Item>();

			handler.events["IsBoss"] = new SEventFBool<NPC>();
			handler.events["RequiresAttaching"] = new SEventFBool<NPC>();
			handler.events["IsUnsafeToSpawn"] = new SEventFBool<NPC>();
			handler.events["BuffHasTimer"] = new SEventFBool<int>();

			handler.funcs["IsBoss"] = IsBoss;
			handler.funcs["RequiresAttaching"] = RequiresAttaching;
			handler.funcs["IsUnsafeToSpawn"] = IsUnsafeToSpawn;
			handler.funcs["BuffHasTimer"] = BuffHasTimer;

			handler.funcs["PutItem"] = (Muple<Item> item, Item[] container) =>
				PutItem(ref item.Item, container);
			handler.funcs["PutItemRange"] = (Muple<Item> item, Item[] container, int rangeStart, int rangeEnd) =>
				PutItem(ref item.Item, container, rangeStart, rangeEnd);

			handler.funcs["CopyFurther"] = CopyFurther;
			handler.actions["CopyFutherInto"] = CopyFurtherInto;

			//SFrame.LoadAll();
		}

		public override void OnAllModsLoaded()
		{
			handler.events["MenuStateChanged"] += (menu) => {
				/*if (menu)
				{
					SFrame.DestroyAll(false);
					SFrame.SaveAll();
					SFrame.DestroyAll(true);
					SPopupMenu.menus.Clear();
				}*/
			};

			handler.events["IsBoss"] += (npc) => {
				if (npc.boss) return true;
				if (npc.type >= 13 && npc.type <= 15) return true; //Eater of Worlds
				if (npc.type >= 134 && npc.type <= 136) return true; //The Destroyer
				if (npc.type == 325 || npc.type == 327) return true; //Mourning Wood, Pumpking
				if (npc.type >= 344 && npc.type <= 346) return true; //Everscream, Ice Queen, Santa-NK1
				return null;
			};
			handler.events["RequiresAttaching"] += (npc) => {
				if (npc.type == 43 || npc.type == 56 || npc.type == 101 || npc.type == 175 || npc.type == 259 || npc.type == 260) return true;
				return null;
			};
			handler.events["IsUnsafeToSpawn"] += (npc) => {
				if (npc.type == 263) return true;
				return null;
			};
			handler.events["BuffHasTimer"] += (type) => {
				if (Main.buffNoTimeDisplay[type]) return false;
				if (Main.vanityPet[type] || Main.lightPet[type]) return false;
				if (noTimerBuffs.Contains(type)) return false;
				if (!Main.localPlayer.honeyWet && type == 48) return true;
				return null;
			};
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (wasInMenu != Main.gameMenu)
			{
				wasInMenu = Main.gameMenu;
				((SEvent<bool>)handler.events["MenuStateChanged"]).Call(wasInMenu);
			}
		}

		public override object OnModCall(ModBase mod, params object[] args)
		{
			return handler.OnModCall(mod, args) ?? base.OnModCall(mod, args);
		}

		public bool IsBoss(NPC npc)
		{
			return ((SEventFBool<NPC>)handler.events["IsBoss"]).Call(npc) ?? false;
		}
		public bool RequiresAttaching(NPC npc)
		{
			return ((SEventFBool<NPC>)handler.events["RequiresAttaching"]).Call(npc) ?? false;
		}
		public bool IsUnsafeToSpawn(NPC npc)
		{
			return ((SEventFBool<NPC>)handler.events["IsUnsafeToSpawn"]).Call(npc) ?? false;
		}
		public bool BuffHasTimer(int type)
		{
			return ((SEventFBool<int>)handler.events["BuffHasTimer"]).Call(type) ?? true;
		}

		public bool PutItem(ref Item item, Item[] items, int rangeStart, int rangeEnd)
		{
			return PutItem(ref item, items, new Tuple<int, int>(rangeStart, rangeEnd));
		}
		public bool PutItem(ref Item item, Item[] items, Tuple<int, int> range = null)
		{
			if (item.IsBlank()) return false;
			if (range == null) range = new Tuple<int,int>(0, items.Length - 1);
			for (int i = range.Item1; i <= range.Item2; i++)
			{
				Item item2 = items[i];
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
			for (int item2id = range.Item1; item2id <= range.Item2; item2id++)
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

		public void CopyFurtherInto(BinBuffer source, BinBuffer into)
		{
			int pos = source.Pos;
			into.Write(source);
			into.Pos = 0;
			source.Pos = pos;
		}
		public BinBuffer CopyFurther(BinBuffer source)
		{
			BinBuffer bb = new BinBuffer();
			CopyFurtherInto(source, bb);
			return bb;
		}
	}
}