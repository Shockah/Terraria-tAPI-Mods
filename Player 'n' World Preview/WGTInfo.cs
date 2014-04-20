using TAPI;
using Terraria;

namespace Shockah.PNWPreview
{
	public class WGTInfo : WorldGenTask
	{
		public WGTInfo(ModBase modBase) : base(modBase.modName) { }
		
		public override void Generate()
		{
			WorldInfo wi = new WorldInfo();
			wi.width = Main.maxTilesX;
			wi.height = Main.maxTilesY;
			wi.corruption = WorldGen.corruption;
			wi.crimson = WorldGen.crimson;
			wi.oreEasy[(int)WorldInfo.Ore.Copper] = WorldGen.copperBar == 20;
			wi.oreEasy[(int)WorldInfo.Ore.Tin] = WorldGen.copperBar == 703;
			wi.oreEasy[(int)WorldInfo.Ore.Iron] = WorldGen.ironBar == 22;
			wi.oreEasy[(int)WorldInfo.Ore.Lead] = WorldGen.ironBar == 704;
			wi.oreEasy[(int)WorldInfo.Ore.Silver] = WorldGen.silverBar == 21;
			wi.oreEasy[(int)WorldInfo.Ore.Tungsten] = WorldGen.silverBar == 705;
			wi.oreEasy[(int)WorldInfo.Ore.Gold] = WorldGen.goldBar == 19;
			wi.oreEasy[(int)WorldInfo.Ore.Platinum] = WorldGen.goldBar == 706;
			WorldGen.oreTier1 = (wi.oreHard[0] = WorldGen.genRand.Next(2) == 0) ? 221 : 107;
			WorldGen.oreTier2 = (wi.oreHard[1] = WorldGen.genRand.Next(2) == 0) ? 222 : 108;
			WorldGen.oreTier3 = (wi.oreHard[2] = WorldGen.genRand.Next(2) == 0) ? 223 : 111;
			wi.townNPCs.Add("Vanilla:Guide");
			wi.read = true;
			MBase.infos[Main.worldName] = wi;
			MBase.shouldSave = true;
		}
	}
}