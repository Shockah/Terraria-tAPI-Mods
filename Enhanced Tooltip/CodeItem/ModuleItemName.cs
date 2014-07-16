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
	public class ModuleItemName : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			string stackText = null;
			switch ((string)options["itemNameStackFormat"].Value)
			{
				case "(100)": if (item.stack > 1) stackText = "(" + item.stack + ")"; break;
				case "(100/999)": stackText = "(" + item.stack + "/" + item.maxStack + ")"; break;
				case "x100": if (item.stack > 1) stackText = "x" + item.stack; break;
				case "x100/999": stackText = "x" + item.stack + "/" + item.maxStack; break;
				default: break;
			}

			Color color = Color.White;
			switch ((string)options["itemNameStackColor"].Value)
			{
				case "Rarity": color = item.GetRarityColor(); break;
				case "Stack": float f = 1f * item.stack / item.maxStack; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
				default: break;
			}

			float scale = (float)options["itemNameScale"].Value;
			if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(item.GetRarityColor(), item.AffixName(), stackText == null ? "" : CText(" ", color, stackText)), scale);
			if (style == ETipStyle.TwoCols) tip += new STooltip.Line(CText(item.GetRarityColor(), item.AffixName()), CText(stackText == null ? "" : CText(color, stackText)), scale);
		}
	}
}