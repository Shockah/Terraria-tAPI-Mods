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
	public class ModuleItemTipText : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.tooltips != null)
			{
				Player player = Main.localPlayer;
				float tipScale = (float)options["itemTipScale"].Value;
				List<string> effective = new List<string>(item.tooltips);
				if (item.modEntities.Count != 0) item.ModifyToolTip(player, effective);
				foreach (string tt in effective) tip += new STooltip.Line(tt, tipScale);
			}
		}
	}
}