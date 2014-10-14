using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.AccSlots
{
	[GlobalMod] public class MNPC : ModNPC
	{
		public const int MAX_SHOP_SLOTS = 8;

		public static readonly int[] shopNPCs = new int[] {
			17, //Merchant
			20, //Dryad
			107, //Goblin Tinkerer
			228, //Witch Doctor
			124, //Mechanic
			108, //Wizard
			178, //Steampunker
			209, //Cyborg
		};
		
		public override void PostSetupShop(Chest chest, ref int lastIndex)
		{
			if (MBase.me.optUnlockMode != "Shopping")
			{
				return;
			}

			for (int i = 0; i < shopNPCs.Length; i++)
			{
				if (npc.type == shopNPCs[i])
				{
					chest.item[lastIndex++].SetDefaults(ID.ITEM_SLOT[i].name);
					break;
				}
			}
		}
	}
}