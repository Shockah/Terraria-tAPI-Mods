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
	public class ModuleItemDefense : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.defense != 0)
			{
				Color color = Color.White;
				switch ((string)options["itemDefenseColor"].Value)
				{
					case "Green/Red": color = item.defense > 0 ? Color.Lime : Color.Red; break;
					default: break;
				}

				if (style == ETipStyle.Vanilla) tip += CText(color, item.defense, "#; defense");
				if (style == ETipStyle.TwoCols) tip += new string[] { "Defense:", CText(color, item.defense) };
			}
		}
	}
}