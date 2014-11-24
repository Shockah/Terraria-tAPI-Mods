using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.InvTweaks
{
	public class MWorld : ModWorld
	{
		public override void Initialize()
		{
			foreach (SlotAction action in ((MInterface)modBase.modInterface).actions)
				action.Load(modBase.options);
		}
	}
}