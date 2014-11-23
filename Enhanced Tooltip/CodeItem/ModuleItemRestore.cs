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
			if (HideSocial(options, item)) return;
			
			bool optRestoreColorEffect = (bool)options["itemRestoreColor"].Value;
			bool optRestoreColorType = (bool)options["itemRestoreTypeColor"].Value;
			bool gray = GraySocial(options, item);
			if (item.healLife > 0)
			{
				if (style == ETipStyle.Vanilla) tip += CText("Restores ", gray ? Color.DarkGray : (optRestoreColorEffect ? Color.Lime : Color.White), item.healLife, optRestoreColorType ? Color.Red : Color.White, " life");
				if (style == ETipStyle.TwoCols) tip += new string[] { CText(optRestoreColorType ? Color.Red : Color.White, "Life#; restore:"), CText(gray ? Color.DarkGray : (optRestoreColorEffect ? Color.Lime : Color.White), item.healLife) };
			}
			if (item.healMana > 0)
			{
				if (style == ETipStyle.Vanilla) tip += CText("Restores ", gray ? Color.DarkGray : (optRestoreColorEffect ? Color.Lime : Color.White), item.healMana, optRestoreColorType ? Color.DeepSkyBlue : Color.White, " mana");
				if (style == ETipStyle.TwoCols) tip += new string[] { CText(optRestoreColorType ? Color.DeepSkyBlue : Color.White, "Mana#; restore:"), CText(gray ? Color.DarkGray : (optRestoreColorEffect ? Color.Lime : Color.White), item.healMana) };
			}
		}
	}
}