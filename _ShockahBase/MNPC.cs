using System;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	[GlobalMod] public class MNPC : ModNPC
	{
		public uint[] cacheItems;

		public MNPC(ModBase modBase, NPC npc) : base(modBase, npc) { }

		public override bool PreNPCLoot()
		{
			cacheItems = new uint[Main.item.Length - 1];
			for (int i = 0; i < cacheItems.Length; i++)
			{
				if (Main.item[i].IsBlank()) continue;
				MItem mitem = Main.item[i].GetSubClass<MItem>();
				if (mitem == null) continue;
				cacheItems[i] = mitem.myId;
			}

			return true;
		}

		public override void PostNPCLoot()
		{
			for (int i = 0; i < cacheItems.Length; i++)
			{
				if (Main.item[i].IsBlank()) continue;
				MItem mitem = Main.item[i].GetSubClass<MItem>();
				if (mitem == null) continue;
				if (mitem.myId != cacheItems[i])
				{
					foreach (Action<NPC, Item> h in SBase.EventNPCLoot) h(npc, Main.item[i]);
				}
			}
		}
	}
}