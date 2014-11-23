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
	public class ModuleItemBait : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (HideSocial(options, item)) return;
			
			if (item.bait > 0)
			{
				Color color = Color.White;
				switch ((string)options["itemToolPowerColor"].Value)
				{
					case "Green": color = Color.Lime; break;
					case "Power": float f = 1f * item.bait / MBase.me.maxPowerBait; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
					default: break;
				}
				if (item.bait == 666 && color != Color.White)
					color = Color.Lerp(Color.DarkViolet, Color.White, (float)Math.Sin(Math.PI * Environment.TickCount / 500));
				if (GraySocial(options, item)) color = Color.DarkGray;

				if (style == ETipStyle.Vanilla) tip += CText(color, item.bait, "%#; bait power");
				if (style == ETipStyle.TwoCols) tip += new string[] { "Bait power:", CText(color, item.bait, "%") };
			}
		}
	}
}