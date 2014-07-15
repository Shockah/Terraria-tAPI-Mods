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
	public class ModuleItemSpeed : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.damage > 0 && !item.notAmmo && item.useStyle > 0 && item.useAnimation > 0)
			{
				string speedText = null;
				if (item.useAnimation <= 8) speedText = "Insanely fast";
				else if (item.useAnimation <= 20) speedText = "Very fast";
				else if (item.useAnimation <= 25) speedText = "Fast";
				else if (item.useAnimation <= 30) speedText = "Average";
				else if (item.useAnimation <= 35) speedText = "Slow";
				else if (item.useAnimation <= 45) speedText = "Very slow";
				else if (item.useAnimation <= 55) speedText = "Extremely slow";
				else speedText = "Snail";

				Color color = Color.White;
				switch ((string)options["itemSpeedColor"].Value)
				{
					case "Speed": float f = 1f * Math.Min(item.useAnimation, 55) / 55; color = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, f); break;
					default: break;
				}

				if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(color, speedText, (bool)options["itemSpeedDetails"].Value ? " (" + item.useAnimation + "/60s)" : "", "#; speed"));
				if (style == ETipStyle.TwoCols) tip += new STooltip.Line("Speed:", CText(color, speedText.ToLower(), (bool)options["itemSpeedDetails"].Value ? " (" + item.useAnimation + "/60s)" : ""));
			}
		}
	}
}