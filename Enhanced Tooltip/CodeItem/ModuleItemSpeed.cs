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
			if (HideSocial(options, item)) return;
			
			if (item.damage > 0 && !item.notAmmo && item.useStyle > 0 && item.useAnimation > 0)
			{
				Item itemDef = item.def.item;
				
				int bstats = BaseStats(options, itemDef.useAnimation == item.useAnimation);
				StringBuilder sbv = new StringBuilder();
				if ((bstats & 1) != 0) FormatValue(sbv, itemDef.useAnimation, style, options, item);
				if (bstats == 3) sbv.Append("#; -> ");
				if ((bstats & 2) != 0) FormatValue(sbv, item.useAnimation, style, options, item);

				if (style == ETipStyle.Vanilla) tip += "" + sbv + "#; speed";
				if (style == ETipStyle.TwoCols) tip += new string[] { "Speed:", "" + sbv };
			}
		}

		private void FormatValue(StringBuilder sb, float v, ETipStyle style, OptionList options, Item item)
		{
			string speedText = null;
			if (v <= 8) speedText = "Insanely fast";
			else if (v <= 20) speedText = "Very fast";
			else if (v <= 25) speedText = "Fast";
			else if (v <= 30) speedText = "Average";
			else if (v <= 35) speedText = "Slow";
			else if (v <= 45) speedText = "Very slow";
			else if (v <= 55) speedText = "Extremely slow";
			else speedText = "Snail";
			if (style == ETipStyle.TwoCols) speedText = speedText.ToLower();

			Color color = Color.White;
			switch ((string)options["itemSpeedColor"].Value)
			{
				case "Speed": float f = 1f * Math.Min(v, 55) / 55; color = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, f); break;
				default: break;
			}
			if (GraySocial(options, item)) color = Color.DarkGray;

			switch ((string)options["itemSpeedDetails"].Value)
			{
				case "Text": sb.Append(CText(color, speedText)); break;
				case "Value": sb.Append(CText(color, v)); break;
				case "Value/60s": sb.Append(CText(color, v, "/60s")); break;
				case "Text and value": sb.Append(CText(color, speedText, " (", v, ")")); break;
				case "Text and value/60s": sb.Append(CText(color, speedText, " (", v, "/60s)")); break;
				default: break;
			}
		}
	}
}