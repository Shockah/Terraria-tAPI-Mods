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
			foreach (Func<STooltip> h in SBase.EventSTooltipDraw)
			{
				STooltip ttip2 = h();
				if (ttip2 != null && ttip2.lines.Count != 0)
				{
					ttip = ttip2;
					break;
				}
			}
			if (ttip == null && STooltip.global.lines.Count != 0) ttip = STooltip.global;
			if (ttip != null)
			{
				ttip.Draw(sb, Main.mouse + new Vector2(10, 10));
				ttip.Clear();
			}
			else if (!string.IsNullOrEmpty(SBase.tip))
			{
				ttip = STooltip.global;
				ttip += SBase.tip;
				ttip.Draw(sb, Main.mouse + new Vector2(10, 10));
				ttip.Clear();
				SBase.tip = null;
			}
		}
	}
}