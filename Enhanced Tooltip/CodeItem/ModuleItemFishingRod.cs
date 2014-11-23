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
	public class ModuleItemFishingRod : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (HideSocial(options, item)) return;
			
			if (item.pick > 0)
			{
				Color color = Color.White;
				switch ((string)options["itemToolPowerColor"].Value)
				{
					case "Green": color = Color.Lime; break;
					case "Power": float f = 1f * item.pick / MBase.me.maxPowerRod; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
					default: break;
				}
				if (GraySocial(options, item)) color = Color.DarkGray;

				if (style == ETipStyle.Vanilla) tip += CText(color, item.pick, "%#; fishing power");
				if (style == ETipStyle.TwoCols) tip += new string[] { "Fishing power:", CText(color, item.pick, "%") };
			}
		}
	}
}