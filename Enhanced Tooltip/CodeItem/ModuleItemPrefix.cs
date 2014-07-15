using Microsoft.Xna.Framework;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip.ModuleItem
{
	public class ModuleItemPrefix : Module<Item>
	{
		public static readonly Color
			COLOR_POSITIVE = new Color(120, 190, 120),
			COLOR_NEGATIVE = new Color(190, 120, 120);
		
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (!item.prefix.Equals(Prefix.None))
			{
				foreach (Tuple<string, bool> t in item.prefix.TooltipText(item))
				{
					tip += new STooltip.Line(CText(t.Item2 ? COLOR_POSITIVE : COLOR_NEGATIVE, t.Item1));
				}
			}
		}
	}
}