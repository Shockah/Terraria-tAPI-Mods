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
	public class ModuleItemAmmo : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.createWall <= 0 && item.createTile <= -1 && item.name != "Xmas decorations" && item.ammo > 0 && !item.notAmmo)
			{
				if (item.ammo == 1 || item.ammo == 323) tip += new STooltip.Line("Ammo (arrow)");
				else if (item.ammo == 14 || item.ammo == 311) tip += new STooltip.Line("Ammo (bullet)");
				else if (item.ammo == 771 || item.ammo == 246 || item.useAmmo == 312) tip += new STooltip.Line("Ammo (rocket)");
				else tip += new STooltip.Line("Ammo");
			}
		}
	}
}