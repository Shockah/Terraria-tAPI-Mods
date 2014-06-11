using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Insight
{
	public class MBase : ModBase
	{
		public static ModBase me { get; private set; }

		public override void OnLoad()
		{
			me = this;
		}
	}
}