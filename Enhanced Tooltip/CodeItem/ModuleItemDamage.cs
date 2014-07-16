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
	public class ModuleItemDamage : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.damage > 0 && !item.notAmmo)
			{
				Item itemDef = item.Def();
				Player player = Main.localPlayer;
				StringBuilder sb = new StringBuilder();
				float itemDamage = item.damage;

				if (item.melee)
				{
					if (sb.Length != 0) sb.Append(", ");
					sb.Append("melee");
					itemDamage *= player.meleeDamage;
				}
				if (item.ranged)
				{
					if (sb.Length != 0) sb.Append(", ");
					sb.Append("ranged");
					itemDamage *= player.rangedDamage;
					if (item.useAmmo == 1 || item.useAmmo == 323 || item.ammo == 1 || item.ammo == 323)
					{
						if (item.useAmmo == 1 || item.useAmmo == 323) itemDamage *= player.arrowDamage;
						sb.Append(" (arrow)");
					}
					else if (item.useAmmo == 14 || item.useAmmo == 311 || item.ammo == 14 || item.ammo == 311)
					{
						if (item.useAmmo == 14 || item.useAmmo == 311) itemDamage *= player.bulletDamage;
						sb.Append(" (bullet)");
					}
					else if (item.useAmmo == 771 || item.useAmmo == 246 || item.useAmmo == 312 || item.ammo == 771 || item.ammo == 246 || item.ammo == 312)
					{
						if (item.useAmmo == 771 || item.useAmmo == 246 || item.useAmmo == 312) itemDamage *= player.rocketDamage;
						sb.Append(" (rocket)");
					}
				}
				if (item.magic)
				{
					if (sb.Length != 0) sb.Append(", ");
					sb.Append("magic");
					itemDamage *= player.magicDamage;
				}
				if (item.summon)
				{
					if (sb.Length != 0) sb.Append(", ");
					sb.Append("summon");
					itemDamage *= player.minionDamage;
				}
				if (sb.Length != 0) sb.Append(" ");

				int bstats = BaseStats(options, itemDef.damage == itemDamage);
				StringBuilder sbv = new StringBuilder();
				if ((bstats & 1) != 0) FormatValue(sbv, itemDef.damage, player, itemDef, style, options);
				if (bstats == 3) sbv.Append("#; -> ");
				if ((bstats & 2) != 0) FormatValue(sbv, itemDamage, player, item, style, options);

				if (style == ETipStyle.Vanilla) tip += "" + sbv + "#; " + sb + "damage";
				if (style == ETipStyle.TwoCols)
				{
					string s = sb.ToString();
					if (!string.IsNullOrEmpty(s)) s = ("" + s[0]).ToUpper() + s.Substring(1);
					s = string.IsNullOrEmpty(s) ? "Damage" : s + "damage";
					tip += new string[] { s + ":", "" + sbv };
				}
			}
		}

		private void FormatValue(StringBuilder sb, float v, Player player, Item item, ETipStyle style, OptionList options)
		{
			Color color = Color.White;
			switch ((string)options["itemDamageColor"].Value)
			{
				case "Viability":
					float min = Math2.Min(player.meleeDamage, player.rangedDamage, player.magicDamage, player.minionDamage);
					float max = Math2.Max(player.meleeDamage, player.rangedDamage, player.magicDamage, player.minionDamage);
					if (item.useAmmo == 1 || item.useAmmo == 323 || item.useAmmo == 14 || item.useAmmo == 311 || item.useAmmo == 771 || item.useAmmo == 246 || item.useAmmo == 312)
					{
						min = Math2.Min(min, player.rangedDamage * player.arrowDamage, player.rangedDamage * player.bulletDamage, player.rangedDamage * player.rocketDamage);
						max = Math2.Max(max, player.rangedDamage * player.arrowDamage, player.rangedDamage * player.bulletDamage, player.rangedDamage * player.rocketDamage);
					}
					float cur = -1f;
					if (item.melee && cur < player.meleeDamage) cur = player.meleeDamage;
					if (item.ranged && cur < player.rangedDamage)
					{
						cur = player.rangedDamage;
						if (item.useAmmo == 1 || item.useAmmo == 323) cur *= player.arrowDamage;
						if (item.useAmmo == 14 || item.useAmmo == 311) cur *= player.bulletDamage;
						if (item.useAmmo == 771 || item.useAmmo == 246 || item.useAmmo == 312) cur *= player.rocketDamage;
					}
					if (item.magic && cur < player.magicDamage) cur = player.magicDamage;
					if (item.summon && cur < player.minionDamage) cur = player.minionDamage;
					float f = (cur - min) / (max - min);
					color = cur == -1f ? Color.White : DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f);
					break;
				default: break;
			}

			sb.Append(CText(color, (int)v));
		}
	}
}