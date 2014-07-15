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
				Player player = Main.localPlayer;

				int itemCrit = item.crit;
				if (item.melee) itemCrit = player.meleeCrit - player.heldItem.crit + itemCrit;
				else if (item.ranged) itemCrit = player.rangedCrit - player.heldItem.crit + itemCrit;
				else if (item.magic) itemCrit = player.magicCrit - player.heldItem.crit + itemCrit;

				if (itemCrit > 0)
				{
					Color color = Color.White;
					switch ((string)options["itemCritColor"].Value)
					{
						case "Chance": float f = itemCrit / 100f; color = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
						default: break;
					}

					if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(color, (int)itemCrit, "%#; critical strike chance"));
					if (style == ETipStyle.TwoCols) tip += new STooltip.Line("Critical strike chance:", CText(color, (int)itemCrit, "%"));
				}
			}
		}
	}
}