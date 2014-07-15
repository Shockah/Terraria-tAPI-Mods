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
			if (item.damage > 0 && !item.notAmmo)
			{
				Player player = Main.localPlayer;
				
				float knockback = item.knockBack;
				if (player.kbGlove) knockback *= 1.7f;
				if (item.ranged && player.armorSteath) knockback *= 1f + (1f - player.stealth) * 0.5f;
				if (item.summon) knockback += player.minionKB;

				string knockbackText = null;
				if (knockback == 0f) knockbackText = "No";
				else if (knockback <= 1.5f) knockbackText = "Extremely weak";
				else if (knockback <= 3f) knockbackText = "Very weak";
				else if (knockback <= 4f) knockbackText = "Weak";
				else if (knockback <= 6f) knockbackText = "Average";
				else if (knockback <= 7f) knockbackText = "Strong";
				else if (knockback <= 9f) knockbackText = "Very strong";
				else if (knockback <= 11f) knockbackText = "Extremely strong";
				else knockbackText = "Insane";

				Color color = Color.White;
				switch ((string)options["itemKnockbackColor"].Value)
				{
					case "Knockback": float f = Math.Min(knockback, 11f) / 11f; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
					default: break;
				}

				if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(color, knockbackText, "#; knockback"));
				if (style == ETipStyle.TwoCols) tip += new STooltip.Line("Knockback:", CText(color, knockbackText.ToLower()));
			}
		}
	}
}