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
	public class ModuleItemCrit : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.damage > 0 && !item.notAmmo && !item.summon)
			{
				Item itemDef = item.Def();
				Player player = Main.localPlayer;

				int itemCrit = item.crit;
				if (item.melee) itemCrit = player.meleeCrit - player.heldItem.crit + itemCrit;
				else if (item.ranged) itemCrit = player.rangedCrit - player.heldItem.crit + itemCrit;
				else if (item.magic) itemCrit = player.magicCrit - player.heldItem.crit + itemCrit;

				if (itemCrit > 0)
				{
					int bstats = BaseStats(options, itemDef.crit == itemCrit);
					StringBuilder sbv = new StringBuilder();
					if ((bstats & 1) != 0) FormatValue(sbv, itemDef.crit, style, options);
					if (bstats == 3) sbv.Append("#; -> ");
					if ((bstats & 2) != 0) FormatValue(sbv, itemCrit, style, options);

					if (style == ETipStyle.Vanilla) tip += "" + sbv + "#; critical strike chance";
					if (style == ETipStyle.TwoCols) tip += new string[] { "Critical strike chance:", "" + sbv };
				}
			}
		}

		private void FormatValue(StringBuilder sb, int v, ETipStyle style, OptionList options)
		{
			Color color = Color.White;
			switch ((string)options["itemCritColor"].Value)
			{
				case "Chance": float f = v / 100f; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
				default: break;
			}

			sb.Append(CText(color, v, "%"));
		}
	}
}