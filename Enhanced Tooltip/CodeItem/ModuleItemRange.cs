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
	public class ModuleItemRange : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.tileBoost != 0)
			{
				Color color = Color.White;
				switch ((string)options["itemToolRangeColor"].Value)
				{
					case "Green/Red": color = item.tileBoost > 0 ? Color.Lime : Color.Red; break;
					default: break;
				}

				if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(color, item.tileBoost > 0 ? "+" : "", item.tileBoost, "#; range"));
				if (style == ETipStyle.TwoCols) tip += new STooltip.Line("Range:", CText(color, item.tileBoost > 0 ? "+" : "", item.tileBoost));
			}
		}
	}
}