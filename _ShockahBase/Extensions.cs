using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public static class Extensions
	{
		public static string ToHex(this Color color, bool includeHash = true, bool includeAlpha = false)
		{
			string[] argb = includeAlpha ?
			new string[] { color.A.ToString("X2"), color.R.ToString("X2"), color.G.ToString("X2"), color.B.ToString("X2") } :
			new string[] { color.R.ToString("X2"), color.G.ToString("X2"), color.B.ToString("X2") };
			return (includeHash ? "#" : string.Empty) + string.Join(string.Empty, argb);
		}

		public static Item Def(this Item item)
		{
			string s = item.name;
			if (s.IndexOf(':') == -1) s = "Vanilla:" + s;
			return Defs.items[s];
		}
	}
}
