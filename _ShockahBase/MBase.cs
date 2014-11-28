using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;
using Microsoft.Xna.Framework;

namespace Shockah.Base
{
	public class MBase : ModBase
	{
		internal static MBase me { get; set; }

		public readonly SExternalHandler handler = new SExternalHandler();
		public readonly SFrameManager frameManager;
		public readonly SPopupMenuManager popupMenuManager;
		public string tip = null;
		public STooltip globalTip = new STooltip();
		protected bool wasInMenu = true;
		protected readonly List<int> noTimerBuffs = new List<int>(new int[] { 19, 27, 28, 34, 37, 38, 40, 41, 42, 43, 45, 49, 60, 62, 64, 67, 68, 81, 82, 83, 90 });

		public SEvent<bool> evMenuStateChanged;
		public SEvent<Player, string, int, Item, Item> evInventoryChanged;
		public SEvent<NPC, int, Item> evNPCLoot;
		public SEvent<int, int, int, Item> evTileLoot;
		public SEvent<SpriteBatch, STooltip, Rectangle> evPreSTooltipDraw, evPostSTooltipDraw;
		public SEventFBool<NPC> evIsBoss, evRequiresAttaching, evIsUnsafeToSpawn, evBuffHasTimer;

		public MBase()
		{
			frameManager = new SFrameManager(this);
			popupMenuManager = new SPopupMenuManager(this);
		}

		public override void OnLoad()
		{
			me = this;

			handler.events["MenuStateChanged"] = evMenuStateChanged = new SEvent<bool>();
			handler.events["InventoryChanged"] = evInventoryChanged = new SEvent<Player, string, int, Item, Item>();
			handler.events["NPCLoot"] = evNPCLoot = new SEvent<NPC, int, Item>();
			handler.events["TileLoot"] = evTileLoot = new SEvent<int, int, int, Item>();

			handler.events["PreSTooltipDraw"] = evPreSTooltipDraw = new SEvent<SpriteBatch, STooltip, Rectangle>();
			handler.events["PostSTooltipDraw"] = evPostSTooltipDraw = new SEvent<SpriteBatch, STooltip, Rectangle>();

			handler.events["IsBoss"] = evIsBoss = new SEventFBool<NPC>();
			handler.events["RequiresAttaching"] = evRequiresAttaching = new SEventFBool<NPC>();
			handler.events["IsUnsafeToSpawn"] = evIsUnsafeToSpawn = new SEventFBool<NPC>();
			handler.events["BuffHasTimer"] = evBuffHasTimer = new SEventFBool<int>();

			handler.funcs["IsBoss"] = new Func<NPC, bool>(IsBoss);
			handler.funcs["RequiresAttaching"] = new Func<NPC, bool>(RequiresAttaching);
			handler.funcs["IsUnsafeToSpawn"] = new Func<NPC, bool>(IsUnsafeToSpawn);
			handler.funcs["BuffHasTimer"] = new Func<int, bool>(BuffHasTimer);

			handler.funcs["PutItem"] = new Func<Muple<Item>, Item[], bool>((Muple<Item> item, Item[] container) =>
				PutItem(ref item.Item, container));
			handler.funcs["PutItemRange"] = new Func<Muple<Item>, Item[], int, int, bool>((Muple<Item> item, Item[] container, int rangeStart, int rangeEnd) =>
				PutItem(ref item.Item, container, rangeStart, rangeEnd));

			handler.funcs["CopyFurther"] = new Func<BinBuffer, BinBuffer>(CopyFurther);
			handler.actions["CopyFutherInto"] = new Action<BinBuffer, BinBuffer>(CopyFurtherInto);

			//SFrame.LoadAll();
		}

		public override void OnUnload()
		{
			me = null;
		}

		public override void OnAllModsLoaded()
		{
			evMenuStateChanged += new Action<bool>((menu) => {
				/*if (menu)
				{
					SFrame.DestroyAll(false);
					SFrame.SaveAll();
					SFrame.DestroyAll(true);
					SPopupMenu.menus.Clear();
				}*/
			});

			evIsBoss += new Func<NPC, bool?>((npc) => {
				if (npc.boss) return true;
				if (npc.type >= 13 && npc.type <= 15) return true; //Eater of Worlds
				if (npc.type >= 134 && npc.type <= 136) return true; //The Destroyer
				if (npc.type == 325 || npc.type == 327) return true; //Mourning Wood, Pumpking
				if (npc.type >= 344 && npc.type <= 346) return true; //Everscream, Ice Queen, Santa-NK1
				return null;
			});
			evRequiresAttaching += new Func<NPC, bool?>((npc) => {
				if (npc.type == 43 || npc.type == 56 || npc.type == 101 || npc.type == 175 || npc.type == 259 || npc.type == 260) return true;
				return null;
			});
			evIsUnsafeToSpawn += new Func<NPC, bool?>((npc) => {
				if (npc.type == 263) return true;
				return null;
			});
			evBuffHasTimer += new Func<int, bool?>((type) => {
				if (Main.buffNoTimeDisplay[type]) return false;
				if (Main.vanityPet[type] || Main.lightPet[type]) return false;
				if (noTimerBuffs.Contains(type)) return false;
				if (!Main.localPlayer.honeyWet && type == 48) return true;
				return null;
			});
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (wasInMenu != Main.gameMenu)
			{
				wasInMenu = Main.gameMenu;
				evMenuStateChanged.Call(wasInMenu);
			}
		}

		public override object OnModCall(ModBase mod, params object[] args)
		{
			return handler.OnModCall(mod, args) ?? base.OnModCall(mod, args);
		}

		public bool IsBoss(NPC npc)
		{
			return evIsBoss.Call(npc) ?? false;
		}
		public bool RequiresAttaching(NPC npc)
		{
			return evRequiresAttaching.Call(npc) ?? false;
		}
		public bool IsUnsafeToSpawn(NPC npc)
		{
			return evIsUnsafeToSpawn.Call(npc) ?? false;
		}
		public bool BuffHasTimer(int type)
		{
			return evBuffHasTimer.Call(type) ?? true;
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