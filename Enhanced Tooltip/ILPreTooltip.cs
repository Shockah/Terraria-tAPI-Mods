using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class ILPreTooltip : InterfaceLayer
	{
		public ILPreTooltip() : base(MBase.me.modName + ":PreTooltip") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			if (Main.hoverItemName != null && Main.hoverItemName != "" && Main.mouseItem.IsBlank())
			{
				Main.localPlayer.showItemIcon = false;
				MBase.me.FillTooltip(Main.hoverItemName);
				Main.mouseText = true;
			}
		}
	}
}