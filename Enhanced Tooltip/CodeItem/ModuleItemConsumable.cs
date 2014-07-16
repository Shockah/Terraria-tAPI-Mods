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
	public class ModuleItemConsumable : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.createWall <= 0 && item.createTile <= -1 && item.name != "Xmas decorations" && (item.ammo <= 0 || item.notAmmo) && item.consumable)
			{
				tip += "Consumable";
			}
		}
	}
}