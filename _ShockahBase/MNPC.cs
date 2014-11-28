using System;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	[GlobalMod] public class MNPC : ModNPC
	{
		public uint[] cacheItems;

		public override bool PreNPCLoot()
		{
			var ev = (SEvent<NPC, int, Item>)((MBase)modBase).handler.events["NPCLoot"];
			if (ev.Count == 0) return true;

			cacheItems = new uint[Main.item.Length - 1];
			for (int i = 0; i < cacheItems.Length; i++)
			{
				if (Main.item[i].IsBlank()) continue;
				cacheItems[i] = Main.item[i].GetSubClass<MItem>().myId;
			}

			return true;
		}

		public override void PostNPCLoot()
		{
			var ev = (SEvent<NPC, int, Item>)((MBase)modBase).handler.events["NPCLoot"];
			if (ev.Count == 0) return;

			for (int i = 0; i < cacheItems.Length; i++)
			{
				if (Main.item[i].IsBlank()) continue;
				if (Main.item[i].GetSubClass<MItem>().myId != cacheItems[i])
					ev.Call(npc, i, Main.item[i]);
			}
		}
	}
}