using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	[GlobalMod] public class MNPC : ModNPC
	{
		public override void Initialize()
		{
			if (npc.type == 68)
			{
				MWorld mw = (MWorld)MBase.me.modWorld;
				if (mw.blockNPCSpawn) npc.active = true;
			}
		}

		[CallPriority(-1000000f)] public override List<int> EditSpawnPool(int x, int y, List<int> pool, Player player)
		{
			MWorld mw = (MWorld)MBase.me.modWorld;
			if (mw.blockNPCSpawn) pool.Clear();
			return base.EditSpawnPool(x, y, pool, player);
		}
	}
}