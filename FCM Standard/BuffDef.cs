using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shockah.FCM.Standard
{
	public class BuffDef
	{
		public readonly int type;
		public readonly string name, noModName;

		public BuffDef(int type, string name)
		{
			this.type = type;
			this.name = name;
			noModName = name.Substring(name.IndexOf(':') + 1);
		}
	}
}