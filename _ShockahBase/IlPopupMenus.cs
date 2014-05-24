using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class IlPopupMenus : InterfaceLayer
	{
		public IlPopupMenus() : base(MBase.me.modName + ":PopupMenus") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			SPopupMenu.RenderAll(sb);
		}
	}
}
