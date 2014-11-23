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
	public class ModuleItemKnockback : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (HideSocial(options, item)) return;
			
			if (item.damage > 0 && !item.notAmmo)
			{
				Item itemDef = item.def.item;
				Player player = Main.localPlayer;
				
				float knockback = item.knockBack;
				if (item.melee) knockback *= player.knockbackMeleeMod;
				knockback *= player.knockbackMod;
				if (item.ranged && player.armorStealth) knockback *= 1f + (1f - player.stealth) * 0.5f;
				if (item.summon) knockback += player.minionKB;

				int bstats = BaseStats(options, itemDef.knockBack == knockback);
				StringBuilder sbv = new StringBuilder();
				if ((bstats & 1) != 0) FormatValue(sbv, itemDef.knockBack, style, options, item);
				if (bstats == 3) sbv.Append("#; -> ");
				if ((bstats & 2) != 0) FormatValue(sbv, knockback, style, options, item);

				if (style == ETipStyle.Vanilla) tip += "" + sbv + "#; knockback";
				if (style == ETipStyle.TwoCols) tip += new string[] { "Knockback:", "" + sbv };
			}
		}

		private void FormatValue(StringBuilder sb, float v, ETipStyle style, OptionList options, Item item)
		{
			string knockbackText = null;
			if (v == 0f) knockbackText = "No";
			else if (v <= 1.5f) knockbackText = "Extremely weak";
			else if (v <= 3f) knockbackText = "Very weak";
			else if (v <= 4f) knockbackText = "Weak";
			else if (v <= 6f) knockbackText = "Average";
			else if (v <= 7f) knockbackText = "Strong";
			else if (v <= 9f) knockbackText = "Very strong";
			else if (v <= 11f) knockbackText = "Extremely strong";
			else knockbackText = "Insane";
			if (style == ETipStyle.TwoCols) knockbackText = knockbackText.ToLower();

			Color color = Color.White;
			switch ((string)options["itemKnockbackColor"].Value)
			{
				case "Knockback": float f = Math.Min(v, 11f) / 11f; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
				default: break;
			}
			if (GraySocial(options, item)) color = Color.DarkGray;

			switch ((string)options["itemKnockbackDetails"].Value)
			{
				case "Text": sb.Append(CText(color, knockbackText)); break;
				case "Value": sb.Append(CText(color, v.ToString("0.0"))); break;
				case "Text and value": sb.Append(CText(color, knockbackText, " (", v.ToString("0.0"), ")")); break;
				default: break;
			}
		}
	}
}