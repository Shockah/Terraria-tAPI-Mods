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
	public class ModuleItemWand : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.tileWand > 0)
			{
				Item itemDef = Defs.items[Defs.itemNames[item.tileWand]];
				if (style == ETipStyle.Vanilla) tip += CText("Consumes ", (bool)options["itemWandItemColor"].Value ? itemDef.GetRarityColor() : Color.White, itemDef.displayName);
				if (style == ETipStyle.TwoCols) tip += new string[] { "Consumes:", CText((bool)options["itemWandItemColor"].Value ? itemDef.GetRarityColor() : Color.White, itemDef.displayName) };
			}
		}
	}
}