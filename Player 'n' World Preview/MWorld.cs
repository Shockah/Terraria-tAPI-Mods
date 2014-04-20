using System;
using System.Collections.Generic;
using System.Threading;
using TAPI;
using Terraria;

namespace Shockah.PNWPreview
{
	public class MWorld : ModWorld
	{
		public static WorldGenTask TaskInfo = null;
		
		public MWorld(ModBase modBase) : base(modBase) { }

		public override void Initialize()
		{
			if (MBase.Current.read) return;
			Thread thread = new Thread(new ThreadStart(() =>
				{
					if (WorldGen.genRand == null) WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
					ILPNWThread.status = 0f;
					WorldInfo wi = MBase.Current;
					wi.width = Main.maxTilesX;
					wi.height = Main.maxTilesY;
					if (WorldGen.oreTier1 == -1) WorldGen.oreTier1 = (wi.oreHard[0] = WorldGen.genRand.Next(2) == 0) ? 221 : 107;
					if (WorldGen.oreTier2 == -1) WorldGen.oreTier2 = (wi.oreHard[1] = WorldGen.genRand.Next(2) == 0) ? 222 : 108;
					if (WorldGen.oreTier3 == -1) WorldGen.oreTier3 = (wi.oreHard[2] = WorldGen.genRand.Next(2) == 0) ? 223 : 111;
					if (!wi.townNPCs.Contains("Vanilla:Guide")) wi.townNPCs.Add("Vanilla:Guide");
					for (int y = Main.maxTilesY - 1; y >= 0; y--)
					{
						if (Main.gameMenu)
						{
							ILPNWThread.status = -1f;
							return;
						}
						ILPNWThread.status = -1f * (y - Main.maxTilesY) / Main.maxTilesY;
						for (int x = 0; x < Main.maxTilesX; x++)
						{
							if (Main.tile[x, y] == null) continue;
							if (Main.tile[x, y].active())
							{
								switch (Main.tile[x, y].type)
								{
									case 7:
										wi.oreEasy[(int)WorldInfo.Ore.Copper] = true;
										break;
									case 6:
										wi.oreEasy[(int)WorldInfo.Ore.Iron] = true;
										break;
									case 9:
										wi.oreEasy[(int)WorldInfo.Ore.Silver] = true;
										break;
									case 8:
										wi.oreEasy[(int)WorldInfo.Ore.Gold] = true;
										break;
									case 22:
										wi.corruption = true;
										break;
									case 166:
										wi.oreEasy[(int)WorldInfo.Ore.Tin] = true;
										break;
									case 167:
										wi.oreEasy[(int)WorldInfo.Ore.Lead] = true;
										break;
									case 168:
										wi.oreEasy[(int)WorldInfo.Ore.Tungsten] = true;
										break;
									case 169:
										wi.oreEasy[(int)WorldInfo.Ore.Platinum] = true;
										break;
									case 204:
										wi.crimson = true;
										break;
								}
							}
						}
					}
					ILPNWThread.status = -1f;
					wi.read = true;
				}
			));
			thread.IsBackground = true;
			thread.Priority = ThreadPriority.BelowNormal;
			thread.Start();
		}

		public override void WorldGenModifyTaskList(List<WorldGenTask> list)
		{
			if (TaskInfo == null) TaskInfo = new WGTInfo(modBase);
			list.Add(TaskInfo);
		}

		public override void PostUpdate()
		{
			WorldInfo wi = MBase.Current;
			foreach (NPC npc in Main.npc)
			{
				if (!npc.active || !npc.townNPC || npc.life <= 0) continue;
				if (npc.type == 37) continue; //Old Man
				string s = npc.type < Main.maxNPCTypes ? "Vanilla:" + npc.type : npc.name;
				if (!wi.townNPCs.Contains(s)) wi.townNPCs.Add(s);
			}
		}
	}
}