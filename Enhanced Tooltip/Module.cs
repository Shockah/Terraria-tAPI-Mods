using Microsoft.Xna.Framework;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.ETooltip
{
	public abstract class Module<T> where T : class
	{
		public static Color DoubleLerp(Color c1, Color c2, Color c3, float f)
		{
			return f < .5f ? Color.Lerp(c1, c2, f * 2f) : Color.Lerp(c2, c3, (f - .5f) * 2f);
		}

		public static string CText(params object[] args)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] is Color) sb.Append(SDrawing.ToColorCode((Color)args[i]));
				else sb.Append(args[i]);
			}
			return sb.ToString();
		}
		
		public abstract void ModifyTip(ETipStyle style, OptionList options, STooltip tip, T t);
	}
}