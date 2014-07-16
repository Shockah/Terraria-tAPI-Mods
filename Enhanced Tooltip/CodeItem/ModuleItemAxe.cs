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
	public class ModuleItemAxe : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.axe > 0)
			{
				Color color = Color.White;
				switch ((string)options["itemToolPowerColor"].Value)
				{
					case "Green": color = Color.Lime; break;
					case "Power": float f = 1f * item.axe / MBase.me.maxPowerAxe; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
					default: break;
				}

				if (style == ETipStyle.Vanilla) tip += CText(color, item.axe * 5, "%#; axe power");
				if (style == ETipStyle.TwoCols) tip += new string[] { "Axe power:", CText(color, item.axe * 5, "%") };
			}
		}
	}
}