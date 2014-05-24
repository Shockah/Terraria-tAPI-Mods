using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.MoveAnything
{
	public class MBase : ModBase
	{
		public static ModBase me = null;

		public override void OnLoad()
		{
			me = this;
		}
		
		public override void OnAllModsLoaded()
		{
			SBase.EventMenuStateChange += (menu) =>
				{
					if (menu) return;
					new FrameLife().Create();
					new FrameMana().Create();
					new FrameBreath().Create();
				};
		}
	}
}