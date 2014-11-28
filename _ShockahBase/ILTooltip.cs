using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class ILTooltip : InterfaceLayer
	{
		public ILTooltip() : base(MBase.me.mod.InternalName + ":Tooltip") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			STooltip ttip = null;
			foreach (Func<STooltip> h in MBase.me.evSTooltipDraw)
			{
				STooltip ttip2 = h();
				if (ttip2 != null && ttip2.lines.Count != 0)
				{
					ttip = ttip2;
					break;
				}
			}
			if (ttip == null && MBase.me.globalTip.lines.Count != 0) ttip = MBase.me.globalTip;
			if (ttip != null)
			{
				ttip.Draw(sb, Main.mouse + new Vector2(10, 10));
				ttip.Clear();
			}
			else if (!string.IsNullOrEmpty(MBase.me.tip))
			{
				ttip = MBase.me.globalTip;
				ttip += MBase.me.tip;
				ttip.Draw(sb, Main.mouse + new Vector2(10, 10));
				ttip.Clear();
				MBase.me.tip = null;
			}
		}
	}
}