using System.Collections.Generic;
using TAPI;
namespace Shockah.PNWPreview
{
	public class WorldInfo
	{
		public static void Save(BinBuffer bb, WorldInfo wi)
		{
			bb.Write((ushort)wi.width);
			bb.Write((ushort)wi.height);
			bb.Write(wi.hardmode);
			bb.Write(wi.corruption);
			bb.Write(wi.crimson);
			for (int i = 0; i < wi.oreEasy.Length; i++) bb.Write(wi.oreEasy[i]);
			for (int i = 0; i < wi.oreHard.Length; i++) bb.Write(wi.oreHard[i]);
			for (int i = 0; i < wi.bosses.Length; i++) bb.Write(wi.bosses[i]);
			bb.Write((ushort)wi.townNPCs.Count);
			foreach (string townNPC in wi.townNPCs) bb.Write(townNPC);
		}

		public static WorldInfo Load(BinBuffer bb)
		{
			int count;
			WorldInfo wi = new WorldInfo();
			wi.width = bb.ReadUShort();
			wi.height = bb.ReadUShort();
			wi.hardmode = bb.ReadBool();
			wi.corruption = bb.ReadBool();
			wi.crimson = bb.ReadBool();
			for (int i = 0; i < wi.oreEasy.Length; i++) wi.oreEasy[i] = bb.ReadBool();
			for (int i = 0; i < wi.oreHard.Length; i++) wi.oreHard[i] = bb.ReadBool();
			for (int i = 0; i < wi.bosses.Length; i++) wi.bosses[i] = bb.ReadBool();
			count = bb.ReadUShort();
			while (count-- > 0) wi.townNPCs.Add(bb.ReadString());
			wi.read = true;
			return wi;
		}
		
		public enum Ore
		{
			Copper,
			Iron,
			Silver,
			Gold,

			Tin,
			Lead,
			Tungsten,
			Platinum,

			Count
		}

		public enum Boss
		{
			EyeOfCthulhu,
			EaterOfWorldsOrBrainOfCthulhu,
			Skeletron,
			QueenBee,

			TheTwins,
			TheDestroyer,
			SkeletronPrime,

			Plantera,
			Golem,

			Count
		}
		
		internal bool read = false;
		public int width = 0, height = 0;
		public bool crimson = false, corruption = false, hardmode = false;
		public bool[] oreEasy = new bool[(int)Ore.Count], oreHard = new bool[3], bosses = new bool[(int)Boss.Count];
		public List<string> townNPCs = new List<string>();
	}
}
