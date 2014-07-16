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
	public class ModuleItemRestore : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			bool optToolRangeColorEffect = (bool)options["itemRestoreColor"].Value;
			bool optToolRangeColorType = (bool)options["itemRestoreTypeColor"].Value;
			if (item.healLife > 0)
			{
				if (style == ETipStyle.Vanilla) tip += CText("Restores ", optToolRangeColorEffect ? Color.Lime : Color.White, item.healLife, optToolRangeColorType ? Color.Red : Color.White, " life");
				if (style == ETipStyle.TwoCols) tip += new string[] { CText(optToolRangeColorType ? Color.Red : Color.White, "Life#; restore:"), CText(optToolRangeColorEffect ? Color.Lime : Color.White, item.healLife) };
			}
			if (item.healMana > 0)
			{
				if (style == ETipStyle.Vanilla) tip += CText("Restores ", optToolRangeColorEffect ? Color.Lime : Color.White, item.healMana, optToolRangeColorType ? Color.DeepSkyBlue : Color.White, " mana");
				if (style == ETipStyle.TwoCols) tip += new string[] { CText(optToolRangeColorType ? Color.DeepSkyBlue : Color.White, "Mana#; restore:"), CText(optToolRangeColorEffect ? Color.Lime : Color.White, item.healMana) };
			}
		}
	}
}