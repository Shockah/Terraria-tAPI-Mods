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
	public class ModuleItemEquipable : Module<Item>
	{
		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0 || item.accessory)
			{
				StringBuilder sb = new StringBuilder();
				if (item.headSlot > 0) { if (sb.Length != 0) sb.Append(", "); sb.Append("head"); }
				if (item.bodySlot > 0) { if (sb.Length != 0) sb.Append(", "); sb.Append("body"); }
				if (item.legSlot > 0) { if (sb.Length != 0) sb.Append(", "); sb.Append("legs"); }
				if (item.accessory) { if (sb.Length != 0) sb.Append(", "); sb.Append("accessory"); }

				if (style == ETipStyle.Vanilla) tip += new STooltip.Line("Equipable" + (item.vanity ? " vanity" : "") + ": " + sb);
				if (style == ETipStyle.TwoCols) tip += new STooltip.Line("Equipable" + (item.vanity ? " vanity" : "") + ":", sb.ToString());
			}
		}
	}
}