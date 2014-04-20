using System.Collections.Generic;
using TAPI;

namespace Shockah.FIRE
{
	public class MWorld : ModWorld
	{
		public static MWorld me = null;

		public List<FIREInstance> instances = new List<FIREInstance>();

		public MWorld(ModBase modBase) : base(modBase)
		{
			me = this;
		}

		public override void PostUpdate()
		{
			for (int i = 0; i < instances.Count; i++)
			{
				if (instances[i].Update()) instances.RemoveAt(i--);
			}
		}
	}
}
