using System;
using System.Collections.Generic;
using System.Text;

using TAPI;

namespace TAPI.BloodyEvil
{
	public class MWorld : ModWorld
	{
		public MWorld(ModBase modBase) : base(modBase) {}
		
		public override void WorldGenPostInit()
		{
			WorldGen.corruption = WorldGen.crimson = true;
		}
	}
}