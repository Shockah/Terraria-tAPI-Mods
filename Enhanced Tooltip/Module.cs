using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public abstract class Module<T>
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
				if (args[i] is Color) sb.Append(Drawing.ToColorCode((Color)args[i]));
				else sb.Append(args[i]);
			}
			return sb.ToString();
		}

		public static int BaseStats(OptionList options, bool valuesEqual)
		{
			string s = (string)options["itemBaseStats"].Value;
			if (s == "Always") return valuesEqual ? 2 : 3;
			else if (s == "Hold Alt") return KState.Special.Alt.Down() ? 1 : 2;
			return 2;
		}

		public static bool HideSocial(OptionList options, Item item)
		{
			string val = (string)options["itemSocialStyle"].Value;
			return item.social && val == "Replace with tip";
		}
		public static bool GraySocial(OptionList options, Item item)
		{
			string val = (string)options["itemSocialStyle"].Value;
			return item.social && (val == "Gray stats" || val == "Extra tip + gray stats");
		}
		public static bool TipSocial(OptionList options, Item item)
		{
			string val = (string)options["itemSocialStyle"].Value;
			return item.social && (val == "Extra tip" || val == "Extra tip + gray stats");
		}
		
		public abstract void ModifyTip(ETipStyle style, OptionList options, STooltip tip, T t);
	}
}