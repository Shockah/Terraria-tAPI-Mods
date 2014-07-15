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
	public class ModuleItemMaterial : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.material && !item.notMaterial)
			{
				tip += new STooltip.Line("Material");
			}
		}
	}
}