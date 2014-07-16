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
	public class ModuleItemManaCost : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.mana > 0)
			{
				Player player = Main.localPlayer;
				if (item.type != 127 || !player.spaceGun)
				{
					Item itemDef = item.Def();
					float itemMana = item.mana;
					itemMana *= player.manaCost;
					if ((int)itemMana > 0)
					{
						int bstats = BaseStats(options, (int)itemMana == itemDef.mana);
						StringBuilder sbv = new StringBuilder();
						if ((bstats & 1) != 0) FormatValue(sbv, itemDef.mana, player, style, options);
						if (bstats == 3) sbv.Append("#; -> ");
						if ((bstats & 2) != 0) FormatValue(sbv, (int)itemMana, player, style, options);
						
						Color color = Color.White;
						float f;
						switch ((string)options["itemManaCostColor"].Value)
						{
							case "Blue": color = Color.DeepSkyBlue; break;
							case "Mana cost": f = 1f * itemMana / MBase.me.maxManaCost; color = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, f); break;
							case "Max mana": f = 1f * itemMana / (player.statManaMax2 / 5); color = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, Math.Min(f, 1f)); break;
							default: break;
						}

						if (style == ETipStyle.Vanilla) tip += CText("Uses ", sbv, (bool)options["itemManaCostTypeColor"].Value ? Color.DeepSkyBlue : Color.White, " mana");
						if (style == ETipStyle.TwoCols) tip += new string[] { CText((bool)options["itemManaCostTypeColor"].Value ? Color.DeepSkyBlue : Color.White, "Mana#; cost:"), "" + sbv };
					}
				}
			}
		}

		private void FormatValue(StringBuilder sb, int v, Player player, ETipStyle style, OptionList options)
		{
			Color color = Color.White;
			float f;
			switch ((string)options["itemManaCostColor"].Value)
			{
				case "Blue": color = Color.DeepSkyBlue; break;
				case "Mana cost": f = 1f * v / MBase.me.maxManaCost; color = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, f); break;
				case "Max mana": f = 1f * v / (player.statManaMax2 / 5); color = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, Math.Min(f, 1f)); break;
				default: break;
			}

			sb.Append(CText(color, v));
		}
	}
}