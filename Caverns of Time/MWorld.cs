using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.CavernsOfTime
{
	public class MWorld : ModWorld
	{
		public static WGTCaverns TaskCaverns = null;

		public int forceChristmas = 0, forceHalloween = 0;
		public int cooldownChristmas = 0, cooldownHalloween = 0;
		public int room1X = -1, room1Y = -1, room2X = -1, room2Y = -1;

		public MWorld(ModBase modBase) : base(modBase) { }

		public override void WorldGenModifyTaskList(List<WorldGenTask> list)
		{
			if (TaskCaverns == null) TaskCaverns = new WGTCaverns(modBase);
			list.Add(TaskCaverns);
		}

		public override void Save(BinBuffer bb)
		{
			bb.WriteX(forceChristmas, forceHalloween);
			bb.WriteX(cooldownChristmas, cooldownHalloween);
			bb.WriteX((ushort)room1X, (ushort)room1Y, (ushort)room2X, (ushort)room2Y);
		}

		public override void Load(BinBuffer bb)
		{
			forceChristmas = bb.ReadInt();
			forceHalloween = bb.ReadInt();
			cooldownChristmas = bb.ReadInt();
			cooldownHalloween = bb.ReadInt();
			room1X = bb.ReadUShort();
			room1Y = bb.ReadUShort();
			room2X = bb.ReadUShort();
			room2Y = bb.ReadUShort();
		}

		public override void PostUpdate()
		{
			int oldForceChristmas = forceChristmas;
			int oldForceHalloween = forceHalloween;

			forceChristmas = Math.Max(forceChristmas - 1, 0);
			forceHalloween = Math.Max(forceHalloween - 1, 0);
			cooldownChristmas = Math.Max(cooldownChristmas - 1, 0);
			cooldownHalloween = Math.Max(cooldownHalloween - 1, 0);

			if (forceChristmas == 0 && oldForceChristmas != 0) Main.NewText("The power to control time decays.", 50, 255, 130);
			if (forceHalloween == 0 && oldForceHalloween != 0) Main.NewText("The power to control time decays.", 50, 255, 130);
		}

		public override bool? CheckChristmas()
		{
			if (forceChristmas > 0) return true;
			return base.CheckChristmas();
		}
		public override bool? CheckHalloween()
		{
			if (forceHalloween > 0) return true;
			return base.CheckHalloween();
		}
	}
}