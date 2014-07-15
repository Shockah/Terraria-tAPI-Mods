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
	public class ModuleItemBuffDuration : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.buffTime > 0)
			{
				Color color = Color.White;
				switch ((string)options["itemBuffDurationColor"].Value)
				{
					case "Green": color = Color.Lime; break;
					default: break;
				}

				if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(color, item.buffTime > 60 ? item.buffTime / (60 * 60) : item.buffTime / 60, " ", item.buffTime > 60 * 60 ? "minute" : "second", "#; duration"));
				if (style == ETipStyle.TwoCols) tip += new STooltip.Line("Duration:", CText(color, item.buffTime > 60 ? item.buffTime / (60 * 60) : item.buffTime / 60, " ", item.buffTime > 60 * 60 ? "minute" : "second", item.buffTime >= (item.buffTime > 60 * 60 ? 2 * 60 * 60 : 2 * 60) ? "s" : ""));
			}
		}
	}
}