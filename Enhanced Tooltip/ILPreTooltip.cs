using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class ILPreTooltip : InterfaceLayer
	{
		public ILPreTooltip() : base(MBase.me.modName + ":PreTooltip") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			if (MBase.me.oneTooltip) return;
			
			STooltip ttip = null;
			bool clearRest = false;
			foreach (Func<STooltip> h in SBase.EventSTooltipDraw)
			{
				STooltip ttip2 = h();
				if (ttip == MBase.tip) continue;
				if (ttip2 != null && ttip2.lines.Count != 0)
				{
					if (clearRest)
					{
						ttip2.Clear();
					}
					else
					{
						ttip = ttip2;
						clearRest = true;
					}
				}
			}
			if (ttip == null && STooltip.global.lines.Count != 0 && STooltip.global != MBase.tip) ttip = STooltip.global;
			if (ttip == MBase.tip) ttip = null;

			if (ttip != null && ttip.lines.Count != 0)
			{
				MBase.me.FillTooltip(ttip);
			}
			else if (!string.IsNullOrEmpty(SBase.tip))
			{
				MBase.me.FillTooltip(SBase.tip);
				SBase.tip = null;
			}
			else if (Main.hoverItemName != null && Main.hoverItemName != "" && Main.mouseItem.IsBlank() && !Main.toolTip.item.IsBlank())
			{
				MBase.me.FillTooltip(Main.toolTip);
			}
		}
	}
}