using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TAPI;

namespace Shockah.ItemRanks
{
	public class MBase : ModBase
	{
		public override object OnModCall(ModBase mod, params object[] args)
		{
			if (args.Length == 2)
			{
				if (args[0] is Item && args[1] is int)
				{
					Item item = (Item)args[0];
					foreach (ModEntity me in item.allSubClasses)
					{
						if (me is MItem)
						{
							MItem mitem = (MItem)me;
							mitem.rank = (int)args[1];
							mitem.SetupRank();
							return null;
						}
					}
				}
			}
			return base.OnModCall(mod, args);
		}
	}
}