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
	public class ModuleItemSetBonus : Module<Item>
	{
		public static readonly Color
			COLOR_POSITIVE = new Color(120, 190, 120),
			COLOR_NEGATIVE = new Color(190, 120, 120);

		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			Player player = Main.localPlayer;
			if (Main.toolTip.wornArmor && player.setBonus != "")
			{
				tip += new STooltip.Line("Set bonus: " + player.setBonus, (float)options["itemSetBonusScale"].Value);
			}
		}
	}
}