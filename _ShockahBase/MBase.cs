using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class MBase : APIModBase
	{
		public static MBase me { get; private set; }
		
		internal bool wasInMenu = true;
		
		public override void OnLoad()
		{
			me = this;
			SFrame.LoadAll();
		}

		public override void OnReload()
		{
			SBase.Clear();
			SFrame.Clear();
			SFrame.LoadAll();
		}

		public override void OnAllModsLoaded()
		{
			SBase.EventMenuStateChange += (menu) =>
				{
					if (menu)
					{
						SFrame.DestroyAll(false);
						SFrame.SaveAll();
						SFrame.DestroyAll(true);
						SPopupMenu.menus.Clear();
					}
				};
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (wasInMenu != Main.gameMenu)
			{
				wasInMenu = Main.gameMenu;
				foreach (Action<bool> h in SBase.EventMenuStateChange) h(wasInMenu);
			}
		}

		public override object OnModCall(ModBase mod, params object[] args)
		{
			if (args.Length == 0)
			{
				return base.OnModCall(mod, args);
			}

			string call = (string)args[0];
			if (call.StartsWith("RegisterEvent") && args == 2)
			{
				call = call.Substring("RegisterEvent".Length);
				switch (call)
				{
					case "MenuStateChange":
					{
						var ev = args[1] as Action<bool>;
						if (ev != null) SBase.EventMenuStateChange += ev;
						break;
					}
					case "InventoryChange":
					{
						var ev = args[1] as Action<Player, string, int, Item, Item>;
						if (ev != null) SBase.EventInventoryChange += ev;
						break;
					}
					case "IsBoss":
					{
						var ev = args[1] as Func<NPC, bool>;
						if (ev != null) SBase.EventIsBoss += ev;
						break;
					}
					case "RequiresAttaching":
					{
						var ev = args[1] as Func<NPC, bool>;
						if (ev != null) SBase.EventRequiresAttaching += ev;
						break;
					}
					case "UnsafeSpawn":
					{
						var ev = args[1] as Func<NPC, bool>;
						if (ev != null) SBase.EventUnsafeSpawn += ev;
						break;
					}
					case "NPCLoot":
					{
						var ev = args[1] as Action<NPC, Item>;
						if (ev != null) SBase.EventNPCLoot += ev;
						break;
					}
					case "TileLoot":
					{
						var ev = args[1] as Action<Point, Item>;
						if (ev != null) SBase.EventTileLoot += ev;
						break;
					}
				}
			}
			else
			{
				bool func = call[0] == '>';
				if (func) call = call.Substring(1);

				switch (call)
				{
					case "BuffHasTimer":
					{
						Func<int, bool> f = SBase.BuffHasTimer;
						return func ? f : f((int)args[2]);
						break;
					}
				}
			}

			return base.OnModCall(mod, args);
		}
	}
}